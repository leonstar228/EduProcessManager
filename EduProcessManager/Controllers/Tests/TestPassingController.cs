using EduProcessManager.Data;
using EduProcessManager.Models;
using EduProcessManager.ViewModels.Tests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduProcessManager.Controllers.Tests;

[Authorize(Roles = "Student")]
public class TestPassingController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public TestPassingController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task<IActionResult> Pass(int id)
    {
        var test = await _db.Tests
            .Include(t => t.Questions)
                .ThenInclude(q => q.AnswerOptions)
            .FirstOrDefaultAsync(t => t.Id == id && t.IsPublished);

        if (test == null)
            return NotFound();

        var now = DateTime.Now;

        if ((test.AvailableFrom != null && test.AvailableFrom > now) ||
            (test.AvailableUntil != null && test.AvailableUntil < now))
        {
            TempData["Error"] = "Тест зараз недоступний для проходження.";
            return RedirectToAction("Index", "Tests");
        }

        var userId = _userManager.GetUserId(User);

        var attemptsCount = await _db.TestAttempts
            .CountAsync(a => a.TestId == id && a.StudentId == userId);

        if (test.MaxAttempts > 0 && attemptsCount >= test.MaxAttempts)
        {
            TempData["Error"] = "Ви вичерпали кількість спроб для цього тесту.";
            return RedirectToAction("Index", "Tests");
        }

        return View(new TestPassViewModel
        {
            TestId = test.Id,
            Title = test.Title,
            TimeLimitMinutes = test.TimeLimitMinutes,
            Questions = test.Questions.ToList(),
            StartedAt = DateTime.Now
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Pass(TestPassViewModel model)
    {
        var test = await _db.Tests
            .Include(t => t.Questions)
                .ThenInclude(q => q.AnswerOptions)
            .FirstOrDefaultAsync(t => t.Id == model.TestId && t.IsPublished);

        if (test == null)
            return NotFound();

        var now = DateTime.Now;

        if ((test.AvailableFrom != null && test.AvailableFrom > now) ||
            (test.AvailableUntil != null && test.AvailableUntil < now))
        {
            TempData["Error"] = "Тест зараз недоступний для проходження.";
            return RedirectToAction("Index", "Tests");
        }

        var userId = _userManager.GetUserId(User);

        var attemptsCount = await _db.TestAttempts
            .CountAsync(a => a.TestId == model.TestId && a.StudentId == userId);

        if (test.MaxAttempts > 0 && attemptsCount >= test.MaxAttempts)
        {
            TempData["Error"] = "Ви вичерпали кількість спроб для цього тесту.";
            return RedirectToAction("Index", "Tests");
        }

        int rawScore = 0;
        int rawMax = test.Questions.Sum(q => q.Points);

        foreach (var q in test.Questions)
        {
            var correct = q.AnswerOptions
                .Where(a => a.IsCorrect)
                .Select(a => a.Id)
                .OrderBy(x => x)
                .ToList();

            var selected = model.SelectedAnswers
                .GetValueOrDefault(q.Id, new List<int>())
                .OrderBy(x => x)
                .ToList();

            if (correct.SequenceEqual(selected))
                rawScore += q.Points;
        }

        int score;
        int maxScore;

        if (test.ScoreScale == TestScoreScale.Percent100)
        {
            score = rawMax == 0 ? 0 : (int)Math.Round(rawScore * 100.0 / rawMax);
            maxScore = 100;
        }
        else if (test.ScoreScale == TestScoreScale.Twelve)
        {
            score = rawMax == 0 ? 0 : (int)Math.Round(rawScore * 12.0 / rawMax);
            maxScore = 12;
        }
        else
        {
            score = rawScore;
            maxScore = rawMax;
        }

        var startedAt = model.StartedAt == default
            ? DateTime.Now.AddMinutes(-test.TimeLimitMinutes)
            : model.StartedAt;

        var attempt = new TestAttempt
        {
            TestId = test.Id,
            StudentId = userId!,
            Score = score,
            MaxScore = maxScore,
            StartedAt = startedAt,
            FinishedAt = DateTime.Now
        };

        _db.TestAttempts.Add(attempt);
        await _db.SaveChangesAsync();

        TempData["Success"] = "Тест завершено. Результат збережено.";

        return RedirectToAction(nameof(Result), new { id = attempt.Id });
    }

    public async Task<IActionResult> Result(int id)
    {
        var attempt = await _db.TestAttempts
            .Include(a => a.Test)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (attempt == null)
            return NotFound();

        if (attempt.StudentId != _userManager.GetUserId(User))
            return Forbid();

        return View(attempt);
    }

    public async Task<IActionResult> MyResults(int? testId)
    {
        var userId = _userManager.GetUserId(User);

        var attempts = _db.TestAttempts
            .Include(a => a.Test)
                .ThenInclude(t => t.Subject)
            .Where(a => a.StudentId == userId)
            .AsQueryable();

        if (testId.HasValue)
            attempts = attempts.Where(a => a.TestId == testId.Value);

        var result = await attempts
            .OrderByDescending(a => a.FinishedAt)
            .ToListAsync();

        ViewBag.TestId = testId;

        return View(result);
    }
}