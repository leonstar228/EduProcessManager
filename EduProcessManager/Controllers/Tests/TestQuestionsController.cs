using EduProcessManager.Data;
using EduProcessManager.Models;
using EduProcessManager.ViewModels.Questions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduProcessManager.Controllers.Tests;

[Authorize(Roles = "Teacher,Admin")]
public class TestQuestionsController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public TestQuestionsController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task<IActionResult> Questions(int testId)
    {
        var test = await _db.Tests
            .Include(t => t.Questions)
                .ThenInclude(q => q.AnswerOptions)
            .FirstOrDefaultAsync(t => t.Id == testId);

        if (test == null)
            return NotFound();

        if (!CanManageTest(test))
            return Forbid();

        return View(test);
    }

    public async Task<IActionResult> CreateQuestion(int testId)
    {
        var test = await _db.Tests.FindAsync(testId);

        if (test == null)
            return NotFound();

        if (!CanManageTest(test))
            return Forbid();

        var model = new QuestionFormViewModel
        {
            TestId = test.Id,
            TestTitle = test.Title,
            Points = 1,
            AllowMultipleAnswers = false
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateQuestion(QuestionFormViewModel model)
    {
        var test = await _db.Tests.FindAsync(model.TestId);

        if (test == null)
            return NotFound();

        if (!CanManageTest(test))
            return Forbid();

        model.TestTitle = test.Title;

        if (!model.AllowMultipleAnswers && model.CorrectOptionIndex.HasValue)
        {
            for (int i = 0; i < model.Options.Count; i++)
                model.Options[i].IsCorrect = i == model.CorrectOptionIndex.Value;
        }

        model.Options = model.Options
            .Where(o => !string.IsNullOrWhiteSpace(o.Text))
            .ToList();

        ValidateQuestionForm(model);

        if (!ModelState.IsValid)
        {
            while (model.Options.Count < 2)
                model.Options.Add(new AnswerOptionFormViewModel());

            return View(model);
        }

        var question = new Question
        {
            TestId = model.TestId,
            Text = model.Text,
            Points = model.Points,
            AllowMultipleAnswers = model.AllowMultipleAnswers
        };

        foreach (var option in model.Options)
        {
            question.AnswerOptions.Add(new AnswerOption
            {
                Text = option.Text,
                IsCorrect = option.IsCorrect
            });
        }

        _db.Questions.Add(question);
        await _db.SaveChangesAsync();

        TempData["Success"] = "Питання додано.";

        return RedirectToAction(nameof(Questions), new { testId = model.TestId });
    }

    public async Task<IActionResult> EditQuestion(int id)
    {
        var question = await _db.Questions
            .Include(q => q.Test)
            .Include(q => q.AnswerOptions)
            .FirstOrDefaultAsync(q => q.Id == id);

        if (question == null)
            return NotFound();

        if (question.Test == null || !CanManageTest(question.Test))
            return Forbid();

        var options = question.AnswerOptions
            .OrderBy(o => o.Id)
            .Select(o => new AnswerOptionFormViewModel
            {
                Id = o.Id,
                Text = o.Text,
                IsCorrect = o.IsCorrect
            })
            .ToList();

        var correctIndex = options.FindIndex(o => o.IsCorrect);

        var model = new QuestionFormViewModel
        {
            Id = question.Id,
            TestId = question.TestId,
            TestTitle = question.Test?.Title ?? string.Empty,
            Text = question.Text,
            Points = question.Points,
            AllowMultipleAnswers = question.AllowMultipleAnswers,
            CorrectOptionIndex = question.AllowMultipleAnswers ? null : correctIndex,
            Options = options
        };

        while (model.Options.Count < 2)
            model.Options.Add(new AnswerOptionFormViewModel());

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditQuestion(QuestionFormViewModel model)
    {
        var question = await _db.Questions
            .Include(q => q.Test)
            .Include(q => q.AnswerOptions)
            .FirstOrDefaultAsync(q => q.Id == model.Id);

        if (question == null)
            return NotFound();

        if (question.Test == null || !CanManageTest(question.Test))
            return Forbid();

        model.TestId = question.TestId;
        model.TestTitle = question.Test?.Title ?? string.Empty;

        if (!model.AllowMultipleAnswers && model.CorrectOptionIndex.HasValue)
        {
            for (int i = 0; i < model.Options.Count; i++)
                model.Options[i].IsCorrect = i == model.CorrectOptionIndex.Value;
        }

        model.Options = model.Options
            .Where(o => !string.IsNullOrWhiteSpace(o.Text))
            .ToList();

        ValidateQuestionForm(model);

        if (!ModelState.IsValid)
        {
            while (model.Options.Count < 2)
                model.Options.Add(new AnswerOptionFormViewModel());

            return View(model);
        }

        question.Text = model.Text;
        question.Points = model.Points;
        question.AllowMultipleAnswers = model.AllowMultipleAnswers;

        _db.AnswerOptions.RemoveRange(question.AnswerOptions);

        foreach (var option in model.Options)
        {
            question.AnswerOptions.Add(new AnswerOption
            {
                Text = option.Text,
                IsCorrect = option.IsCorrect
            });
        }

        await _db.SaveChangesAsync();

        TempData["Success"] = "Питання оновлено.";

        return RedirectToAction(nameof(Questions), new { testId = question.TestId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteQuestion(int id)
    {
        var question = await _db.Questions
            .Include(q => q.Test)
            .Include(q => q.AnswerOptions)
            .FirstOrDefaultAsync(q => q.Id == id);

        if (question == null)
            return NotFound();

        if (question.Test == null || !CanManageTest(question.Test))
            return Forbid();

        var testId = question.TestId;

        _db.AnswerOptions.RemoveRange(question.AnswerOptions);
        _db.Questions.Remove(question);

        await _db.SaveChangesAsync();

        TempData["Success"] = "Питання видалено.";

        return RedirectToAction(nameof(Questions), new { testId });
    }

    private bool CanManageTest(Test test)
    {
        if (User.IsInRole("Admin"))
            return true;

        if (User.IsInRole("Teacher"))
            return test.AuthorId == _userManager.GetUserId(User);

        return false;
    }

    private void ValidateQuestionForm(QuestionFormViewModel model)
    {
        if (string.IsNullOrWhiteSpace(model.Text))
            ModelState.AddModelError(nameof(model.Text), "Введіть текст питання.");

        if (model.Points <= 0)
            ModelState.AddModelError(nameof(model.Points), "Кількість балів має бути більшою за 0.");

        if (model.Options.Count < 2)
            ModelState.AddModelError(nameof(model.Options), "Додайте мінімум два варіанти відповіді.");

        var correctCount = model.Options.Count(o => o.IsCorrect);

        if (model.AllowMultipleAnswers)
        {
            if (correctCount < 2)
                ModelState.AddModelError(nameof(model.Options), "Для питання з декількома правильними відповідями потрібно вибрати мінімум дві правильні відповіді.");
        }
        else
        {
            if (correctCount != 1)
                ModelState.AddModelError(nameof(model.Options), "Для питання з однією правильною відповіддю потрібно вибрати рівно одну правильну відповідь.");
        }
    }
}