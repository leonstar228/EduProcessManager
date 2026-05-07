using EduProcessManager.Data;
using EduProcessManager.Models;
using EduProcessManager.ViewModels.Tests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduProcessManager.Controllers.Tests;

[Authorize]
public class TestsController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public TestsController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var now = DateTime.Now;

        var query = _db.Tests
            .Include(t => t.Subject)
            .Include(t => t.Author)
            .Include(t => t.TestStudentGroups)
                .ThenInclude(x => x.StudentGroup)
            .AsQueryable();

        if (User.IsInRole("Student"))
        {
            var userId = _userManager.GetUserId(User);

            var student = await _db.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            var studentGroupId = student?.StudentGroupId;

            query = query
                .Where(t =>
                    t.IsPublished &&
                    (t.AvailableFrom == null || t.AvailableFrom <= now) &&
                    (t.AvailableUntil == null || t.AvailableUntil >= now) &&
                    (
                        !t.TestStudentGroups.Any() ||
                        (
                            studentGroupId != null &&
                            t.TestStudentGroups.Any(g => g.StudentGroupId == studentGroupId)
                        )
                    ))
                .Include(t => t.Attempts.Where(a => a.StudentId == userId));
        }
        else if (User.IsInRole("Teacher"))
        {
            var userId = _userManager.GetUserId(User);

            query = query
                .Where(t => t.AuthorId == userId)
                .Include(t => t.Attempts);
        }
        else
        {
            query = query.Include(t => t.Attempts);
        }

        var tests = await query
            .OrderBy(t => t.Title)
            .ToListAsync();

        return View(tests);
    }

    [Authorize(Roles = "Teacher,Admin")]
    public async Task<IActionResult> Create()
    {
        var model = new TestFormViewModel
        {
            TimeLimitMinutes = 20,
            MaxAttempts = 1,
            IsPublished = true,
            Groups = await _db.StudentGroups.OrderBy(g => g.Name).ToListAsync()
        };

        return View(model);
    }

    [HttpPost]
    [Authorize(Roles = "Teacher,Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TestFormViewModel model)
    {
        if (!await _db.Subjects.AnyAsync())
            _db.Subjects.Add(new Subject { Name = "Загальна дисципліна", Description = "Автоматично створено" });

        await _db.SaveChangesAsync();

        ModelState.Remove(nameof(model.Groups));

        if (!ModelState.IsValid)
        {
            model.Groups = await _db.StudentGroups.OrderBy(g => g.Name).ToListAsync();
            return View(model);
        }

        var test = new Test
        {
            Title = model.Title,
            Description = model.Description,
            TimeLimitMinutes = model.TimeLimitMinutes,
            MaxAttempts = model.MaxAttempts,
            ScoreScale = model.ScoreScale,
            IsPublished = model.IsPublished,
            AvailableFrom = model.AvailableFrom,
            AvailableUntil = model.AvailableUntil,
            AuthorId = _userManager.GetUserId(User)!,
            SubjectId = await _db.Subjects.Select(x => x.Id).FirstAsync()
        };

        foreach (var groupId in model.SelectedGroupIds.Distinct())
        {
            test.TestStudentGroups.Add(new TestStudentGroup
            {
                StudentGroupId = groupId
            });
        }

        _db.Tests.Add(test);
        await _db.SaveChangesAsync();

        TempData["Success"] = "Тест успішно створено.";

        return RedirectToAction("Questions", "TestQuestions", new { testId = test.Id });
    }

    [Authorize(Roles = "Teacher,Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var test = await _db.Tests
            .Include(t => t.TestStudentGroups)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (test == null)
            return NotFound();

        if (!CanManageTest(test))
            return Forbid();

        var model = new TestFormViewModel
        {
            Id = test.Id,
            Title = test.Title,
            Description = test.Description,
            TimeLimitMinutes = test.TimeLimitMinutes,
            MaxAttempts = test.MaxAttempts,
            ScoreScale = test.ScoreScale,
            IsPublished = test.IsPublished,
            AvailableFrom = test.AvailableFrom,
            AvailableUntil = test.AvailableUntil,
            SelectedGroupIds = test.TestStudentGroups.Select(x => x.StudentGroupId).ToList(),
            Groups = await _db.StudentGroups.OrderBy(g => g.Name).ToListAsync()
        };

        return View(model);
    }

    [HttpPost]
    [Authorize(Roles = "Teacher,Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TestFormViewModel model)
    {
        if (id != model.Id)
            return NotFound();

        var test = await _db.Tests
            .Include(t => t.TestStudentGroups)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (test == null)
            return NotFound();

        if (!CanManageTest(test))
            return Forbid();

        ModelState.Remove(nameof(model.Groups));

        if (!ModelState.IsValid)
        {
            model.Groups = await _db.StudentGroups.OrderBy(g => g.Name).ToListAsync();
            return View(model);
        }

        test.Title = model.Title;
        test.Description = model.Description;
        test.TimeLimitMinutes = model.TimeLimitMinutes;
        test.MaxAttempts = model.MaxAttempts;
        test.ScoreScale = model.ScoreScale;
        test.IsPublished = model.IsPublished;
        test.AvailableFrom = model.AvailableFrom;
        test.AvailableUntil = model.AvailableUntil;

        _db.TestStudentGroups.RemoveRange(test.TestStudentGroups);

        foreach (var groupId in model.SelectedGroupIds.Distinct())
        {
            test.TestStudentGroups.Add(new TestStudentGroup
            {
                TestId = test.Id,
                StudentGroupId = groupId
            });
        }

        await _db.SaveChangesAsync();

        TempData["Success"] = "Тест оновлено.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [Authorize(Roles = "Teacher,Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var test = await _db.Tests
            .Include(t => t.Questions)
                .ThenInclude(q => q.AnswerOptions)
            .Include(t => t.Attempts)
            .Include(t => t.TestStudentGroups)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (test == null)
            return NotFound();

        if (!CanManageTest(test))
            return Forbid();

        if (test.Attempts.Any())
            _db.TestAttempts.RemoveRange(test.Attempts);

        if (test.TestStudentGroups.Any())
            _db.TestStudentGroups.RemoveRange(test.TestStudentGroups);

        foreach (var question in test.Questions)
            _db.AnswerOptions.RemoveRange(question.AnswerOptions);

        _db.Questions.RemoveRange(test.Questions);
        _db.Tests.Remove(test);

        await _db.SaveChangesAsync();

        TempData["Success"] = "Тест видалено.";

        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Teacher,Admin")]
    public async Task<IActionResult> Details(int id)
    {
        var test = await _db.Tests
            .Include(t => t.Subject)
            .Include(t => t.Author)
            .Include(t => t.Attempts)
                .ThenInclude(a => a.Student)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (test == null)
            return NotFound();

        if (!CanManageTest(test))
            return Forbid();

        ViewBag.StudentsCount = test.Attempts
            .Select(a => a.StudentId)
            .Distinct()
            .Count();

        ViewBag.AverageScore = test.Attempts.Any()
            ? Math.Round(test.Attempts.Average(a => a.Score), 1)
            : 0;

        ViewBag.AveragePercent = test.Attempts.Any()
            ? Math.Round(test.Attempts.Average(a => a.Percent), 1)
            : 0;

        return View(test);
    }

    private bool CanManageTest(Test test)
    {
        if (User.IsInRole("Admin"))
            return true;

        if (User.IsInRole("Teacher"))
            return test.AuthorId == _userManager.GetUserId(User);

        return false;
    }
}