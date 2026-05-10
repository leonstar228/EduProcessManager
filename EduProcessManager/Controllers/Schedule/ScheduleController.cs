using EduProcessManager.Data;
using EduProcessManager.Models;
using EduProcessManager.ViewModels.Schedule;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduProcessManager.Controllers.Schedule;

[Authorize(Roles = "Student,Teacher,Admin,SuperAdmin")]
public class ScheduleController : Controller
{
    private readonly ApplicationDbContext _db;

    public ScheduleController(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index(int? groupId, string? search)
    {
        var groupsQuery = _db.StudentGroups.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            groupsQuery = groupsQuery.Where(g => g.Name.Contains(search));

        var groups = await groupsQuery
            .OrderBy(g => g.Name)
            .ToListAsync();

        if (groupId == null && groups.Any())
            groupId = groups.First().Id;

        var lessons = await _db.LessonSchedules
            .Include(x => x.StudentGroup)
            .Where(x => groupId == null || x.StudentGroupId == groupId)
            .OrderBy(x => x.DayOfWeek)
            .ThenBy(x => x.StartTime)
            .ToListAsync();

        var bells = await _db.BellSchedules
            .OrderBy(x => x.LessonNumber)
            .ToListAsync();

        var today = DateTime.Today;
        var diff = ((int)today.DayOfWeek + 6) % 7;
        var weekStart = today.AddDays(-diff);

        var model = new ScheduleIndexViewModel
        {
            SelectedGroupId = groupId,
            Search = search,
            Groups = groups,
            Lessons = lessons,
            Bells = bells,
            WeekStart = weekStart
        };

        return View(model);
    }

    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> Create()
    {
        var model = new LessonScheduleFormViewModel
        {
            Groups = await _db.StudentGroups.OrderBy(g => g.Name).ToListAsync(),
            StartTime = new TimeSpan(8, 30, 0),
            EndTime = new TimeSpan(10, 0, 0)
        };

        return View(model);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,SuperAdmin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(LessonScheduleFormViewModel model)
    {
        await ValidateLesson(model);

        if (!ModelState.IsValid)
        {
            model.Groups = await _db.StudentGroups.OrderBy(g => g.Name).ToListAsync();
            return View(model);
        }

        var lesson = new LessonSchedule
        {
            StudentGroupId = model.StudentGroupId,
            SubjectName = model.SubjectName,
            TeacherName = model.TeacherName,
            Room = model.Room,
            DayOfWeek = model.DayOfWeek,
            StartTime = model.StartTime,
            EndTime = model.EndTime,
            Description = model.Description
        };

        _db.LessonSchedules.Add(lesson);
        await _db.SaveChangesAsync();

        TempData["Success"] = "Заняття додано.";

        return RedirectToAction(nameof(Index), new { groupId = model.StudentGroupId });
    }

    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> Edit(int id)
    {
        var lesson = await _db.LessonSchedules.FindAsync(id);

        if (lesson == null)
            return NotFound();

        var model = new LessonScheduleFormViewModel
        {
            Id = lesson.Id,
            StudentGroupId = lesson.StudentGroupId,
            SubjectName = lesson.SubjectName,
            TeacherName = lesson.TeacherName,
            Room = lesson.Room,
            DayOfWeek = lesson.DayOfWeek,
            StartTime = lesson.StartTime,
            EndTime = lesson.EndTime,
            Description = lesson.Description,
            Groups = await _db.StudentGroups.OrderBy(g => g.Name).ToListAsync()
        };

        return View(model);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,SuperAdmin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, LessonScheduleFormViewModel model)
    {
        if (id != model.Id)
            return NotFound();

        var lesson = await _db.LessonSchedules.FindAsync(id);

        if (lesson == null)
            return NotFound();

        await ValidateLesson(model, id);

        if (!ModelState.IsValid)
        {
            model.Groups = await _db.StudentGroups.OrderBy(g => g.Name).ToListAsync();
            return View(model);
        }

        lesson.StudentGroupId = model.StudentGroupId;
        lesson.SubjectName = model.SubjectName;
        lesson.TeacherName = model.TeacherName;
        lesson.Room = model.Room;
        lesson.DayOfWeek = model.DayOfWeek;
        lesson.StartTime = model.StartTime;
        lesson.EndTime = model.EndTime;
        lesson.Description = model.Description;

        await _db.SaveChangesAsync();

        TempData["Success"] = "Заняття оновлено.";

        return RedirectToAction(nameof(Index), new { groupId = model.StudentGroupId });
    }

    [HttpPost]
    [Authorize(Roles = "Admin,SuperAdmin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var lesson = await _db.LessonSchedules.FindAsync(id);

        if (lesson == null)
            return NotFound();

        var groupId = lesson.StudentGroupId;

        _db.LessonSchedules.Remove(lesson);
        await _db.SaveChangesAsync();

        TempData["Success"] = "Заняття видалено.";

        return RedirectToAction(nameof(Index), new { groupId });
    }

    public async Task<IActionResult> Details(int id)
    {
        var lesson = await _db.LessonSchedules
            .Include(x => x.StudentGroup)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (lesson == null)
        {
            return NotFound();
        }

        return View(lesson);
    }

    private async Task ValidateLesson(LessonScheduleFormViewModel model, int? ignoreId = null)
    {
        if (model.EndTime <= model.StartTime)
        {
            ModelState.AddModelError(nameof(model.EndTime),
                "Кінець заняття має бути пізніше за початок.");
            return;
        }

        if (model.StartTime < new TimeSpan(8, 0, 0))
        {
            ModelState.AddModelError(nameof(model.StartTime),
                "Заняття не може починатися раніше 08:00.");
        }

        if (model.EndTime > new TimeSpan(20, 0, 0))
        {
            ModelState.AddModelError(nameof(model.EndTime),
                "Заняття не може закінчуватися пізніше 20:00.");
        }

        var duration = model.EndTime - model.StartTime;

        if (duration.TotalMinutes < 30)
        {
            ModelState.AddModelError("",
                "Заняття має тривати мінімум 30 хвилин.");
        }

        if (duration.TotalHours > 3)
        {
            ModelState.AddModelError("",
                "Заняття не може тривати більше 3 годин.");
        }

        var minBreak = TimeSpan.FromMinutes(10);

        var lessons = await _db.LessonSchedules
            .Where(x =>
                x.StudentGroupId == model.StudentGroupId &&
                x.DayOfWeek == model.DayOfWeek &&
                (!ignoreId.HasValue || x.Id != ignoreId.Value))
            .ToListAsync();

        foreach (var lesson in lessons)
        {
            var overlaps =
                model.StartTime < lesson.EndTime &&
                model.EndTime > lesson.StartTime;

            var breakBefore =
                model.EndTime <= lesson.StartTime &&
                lesson.StartTime - model.EndTime < minBreak;

            var breakAfter =
                model.StartTime >= lesson.EndTime &&
                model.StartTime - lesson.EndTime < minBreak;

            if (overlaps)
            {
                ModelState.AddModelError("",
                    "Заняття накладається на інше заняття цієї групи.");
                break;
            }

            if (breakBefore || breakAfter)
            {
                ModelState.AddModelError("",
                    "Між заняттями має бути мінімум 10 хвилин перерви.");
                break;
            }
        }
    }
}