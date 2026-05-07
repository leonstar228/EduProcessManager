using EduProcessManager.Data;
using EduProcessManager.Models;
using EduProcessManager.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduProcessManager.Controllers;

[Authorize]
public class TestsController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    public TestsController(ApplicationDbContext db, UserManager<ApplicationUser> userManager) { _db = db; _userManager = userManager; }

    public async Task<IActionResult> Index()
    {
        var tests = await _db.Tests.Include(t => t.Subject).Include(t => t.Author).Where(t => t.IsPublished).ToListAsync();
        return View(tests);
    }

    [Authorize(Roles = "Teacher,Admin,SuperAdmin")]
    public IActionResult Create() => View(new Test { TimeLimitMinutes = 20 });

    [HttpPost, Authorize(Roles = "Teacher,Admin,SuperAdmin")]
    public async Task<IActionResult> Create(Test model)
    {
        model.AuthorId = _userManager.GetUserId(User)!;
        if (!await _db.Subjects.AnyAsync()) _db.Subjects.Add(new Subject { Name = "Загальна дисципліна", Description = "Автоматично створено" });
        await _db.SaveChangesAsync();
        model.SubjectId = await _db.Subjects.Select(x => x.Id).FirstAsync();
        _db.Tests.Add(model);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Pass(int id)
    {
        var test = await _db.Tests.Include(t => t.Questions).ThenInclude(q => q.AnswerOptions).FirstOrDefaultAsync(t => t.Id == id);
        if (test == null) return NotFound();
        return View(new TestPassViewModel { TestId = test.Id, Title = test.Title, TimeLimitMinutes = test.TimeLimitMinutes, Questions = test.Questions.ToList() });
    }

    [HttpPost]
    public async Task<IActionResult> Pass(TestPassViewModel model)
    {
        var test = await _db.Tests.Include(t => t.Questions).ThenInclude(q => q.AnswerOptions).FirstOrDefaultAsync(t => t.Id == model.TestId);
        if (test == null) return NotFound();
        int score = 0, max = test.Questions.Sum(q => q.Points);
        foreach (var q in test.Questions)
        {
            var correct = q.AnswerOptions.Where(a => a.IsCorrect).Select(a => a.Id).OrderBy(x => x).ToList();
            var selected = model.SelectedAnswers.GetValueOrDefault(q.Id, new List<int>()).OrderBy(x => x).ToList();
            if (correct.SequenceEqual(selected)) score += q.Points;
        }
        var attempt = new TestAttempt { TestId = test.Id, StudentId = _userManager.GetUserId(User)!, Score = score, MaxScore = max, StartedAt = DateTime.Now, FinishedAt = DateTime.Now };
        _db.TestAttempts.Add(attempt);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Result), new { id = attempt.Id });
    }

    public async Task<IActionResult> Result(int id)
    {
        var attempt = await _db.TestAttempts.Include(a => a.Test).FirstOrDefaultAsync(a => a.Id == id);
        if (attempt == null) return NotFound();
        return View(attempt);
    }

    [Authorize(Roles = "Teacher,Admin,SuperAdmin")]
    public async Task<IActionResult> Results()
    {
        var attempts = await _db.TestAttempts.Include(a => a.Test).Include(a => a.Student).OrderByDescending(a => a.FinishedAt).ToListAsync();
        return View(attempts);
    }
}
