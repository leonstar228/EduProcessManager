using EduProcessManager.Data;
using EduProcessManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduProcessManager.Controllers.Schedule;

[Authorize(Roles = "Admin,SuperAdmin")]
public class BellScheduleController : Controller
{
    private readonly ApplicationDbContext _db;

    public BellScheduleController(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var bells = await _db.BellSchedules
            .OrderBy(x => x.LessonNumber)
            .ToListAsync();

        return View(bells);
    }

    public IActionResult Create()
    {
        return View(new BellSchedule
        {
            LessonNumber = 1,
            StartTime = new TimeSpan(8, 30, 0),
            EndTime = new TimeSpan(10, 0, 0)
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BellSchedule model)
    {
        await ValidateBell(model);

        if (!ModelState.IsValid)
            return View(model);

        _db.BellSchedules.Add(model);
        await _db.SaveChangesAsync();

        TempData["Success"] = "Дзвінок додано.";

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var bell = await _db.BellSchedules.FindAsync(id);

        if (bell == null)
            return NotFound();

        return View(bell);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, BellSchedule model)
    {
        if (id != model.Id)
            return NotFound();

        await ValidateBell(model, id);

        if (!ModelState.IsValid)
            return View(model);

        var bell = await _db.BellSchedules.FindAsync(id);

        if (bell == null)
            return NotFound();

        bell.LessonNumber = model.LessonNumber;
        bell.StartTime = model.StartTime;
        bell.EndTime = model.EndTime;
        bell.Name = model.Name;

        await _db.SaveChangesAsync();

        TempData["Success"] = "Дзвінок оновлено.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var bell = await _db.BellSchedules.FindAsync(id);

        if (bell == null)
            return NotFound();

        _db.BellSchedules.Remove(bell);

        await _db.SaveChangesAsync();

        TempData["Success"] = "Дзвінок видалено.";

        return RedirectToAction(nameof(Index));
    }

    private async Task ValidateBell(BellSchedule model, int? ignoreId = null)
    {
        if (model.EndTime <= model.StartTime)
        {
            ModelState.AddModelError(nameof(model.EndTime),
                "Час завершення має бути пізніше за початок.");
        }

        if (model.StartTime < new TimeSpan(8, 0, 0))
        {
            ModelState.AddModelError(nameof(model.StartTime),
                "Дзвінки не можуть починатися раніше 08:00.");
        }

        if (model.EndTime > new TimeSpan(20, 0, 0))
        {
            ModelState.AddModelError(nameof(model.EndTime),
                "Дзвінки не можуть закінчуватися пізніше 20:00.");
        }

        var duration = model.EndTime - model.StartTime;

        if (duration.TotalHours > 3)
        {
            ModelState.AddModelError("",
                "Дзвінок не може тривати більше 3 годин.");
        }

        var minBreak = TimeSpan.FromMinutes(10);

        var bells = await _db.BellSchedules
            .Where(x => !ignoreId.HasValue || x.Id != ignoreId.Value)
            .ToListAsync();

        foreach (var bell in bells)
        {
            var overlaps =
                model.StartTime < bell.EndTime &&
                model.EndTime > bell.StartTime;

            var breakBefore =
                model.EndTime <= bell.StartTime &&
                bell.StartTime - model.EndTime < minBreak;

            var breakAfter =
                model.StartTime >= bell.EndTime &&
                model.StartTime - bell.EndTime < minBreak;

            if (overlaps)
            {
                ModelState.AddModelError("",
                    "Цей дзвінок накладається на інший.");
                break;
            }

            if (breakBefore || breakAfter)
            {
                ModelState.AddModelError("",
                    "Між дзвінками має бути мінімум 10 хвилин.");
                break;
            }
        }
    }
}