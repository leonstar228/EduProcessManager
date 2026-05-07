using EduProcessManager.Data;
using EduProcessManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduProcessManager.Controllers;

[Authorize]
public class EventsController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public EventsController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var items = await _db.EduEvents
            .Include(e => e.Registrations)
            .OrderBy(e => e.StartAt)
            .ToListAsync();

        return View(items);
    }

    [Authorize(Roles = "Teacher,Admin")]
    public IActionResult Create()
    {
        var start = DateTime.Now.AddDays(1);

        return View(new EduEvent
        {
            StartAt = start,
            EndAt = start.AddHours(2),
            Capacity = 25,
            IsRegistrationOpen = true
        });
    }

    [HttpPost]
    [Authorize(Roles = "Teacher,Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EduEvent model)
    {
        model.CreatedById = _userManager.GetUserId(User)!;

        if (model.EndAt <= model.StartAt)
        {
            ModelState.AddModelError("EndAt", "Кінець заходу має бути пізніше за початок");
        }

        if (!ModelState.IsValid)
            return View(model);

        _db.EduEvents.Add(model);
        await _db.SaveChangesAsync();

        TempData["Success"] = "Захід успішно створено.";

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int id)
    {
        var item = await _db.EduEvents
            .Include(e => e.Registrations)
            .ThenInclude(r => r.Student)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (item == null)
            return NotFound();

        if (User.IsInRole("Student"))
        {
            var userId = _userManager.GetUserId(User)!;
            ViewBag.MyRegistration = item.Registrations.FirstOrDefault(r => r.StudentId == userId);
        }

        return View(item);
    }

    [Authorize(Roles = "Teacher,Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _db.EduEvents.FindAsync(id);
        if (item == null) return NotFound();
        return View(item);
    }

    [HttpPost]
    [Authorize(Roles = "Teacher,Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EduEvent model)
    {
        if (id != model.Id)
            return NotFound();

        var item = await _db.EduEvents.FindAsync(id);
        if (item == null)
            return NotFound();

        if (model.EndAt <= model.StartAt)
        {
            ModelState.AddModelError("EndAt", "Кінець заходу має бути пізніше за початок");
        }

        if (!ModelState.IsValid)
            return View(model);

        item.Title = model.Title;
        item.Description = model.Description;
        item.Location = model.Location;
        item.StartAt = model.StartAt;
        item.EndAt = model.EndAt;
        item.RegistrationAvailableFrom = model.RegistrationAvailableFrom;
        item.RegistrationAvailableUntil = model.RegistrationAvailableUntil;
        item.Capacity = model.Capacity;
        item.IsRegistrationOpen = model.IsRegistrationOpen;

        await _db.SaveChangesAsync();

        TempData["Success"] = "Дані заходу оновлено.";

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [Authorize(Roles = "Teacher,Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _db.EduEvents
            .Include(e => e.Registrations)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (item == null)
            return NotFound();

        if (item.Registrations.Any())
            _db.EventRegistrations.RemoveRange(item.Registrations);

        _db.EduEvents.Remove(item);
        await _db.SaveChangesAsync();

        TempData["Success"] = "Захід видалено.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [Authorize(Roles = "Student")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(int id)
    {
        var userId = _userManager.GetUserId(User)!;

        var ev = await _db.EduEvents
            .Include(e => e.Registrations)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (ev == null)
        {
            TempData["Error"] = "Захід не знайдено.";
            return RedirectToAction(nameof(Index));
        }

        var now = DateTime.Now;

        if (now > ev.EndAt)
        {
            TempData["Error"] = "Захід уже завершився.";
            return RedirectToAction(nameof(Details), new { id });
        }

        if ((ev.RegistrationAvailableFrom != null && ev.RegistrationAvailableFrom > now) ||
            (ev.RegistrationAvailableUntil != null && ev.RegistrationAvailableUntil < now))
        {
            TempData["Error"] = "Реєстрація зараз недоступна.";
            return RedirectToAction(nameof(Details), new { id });
        }

        if (!ev.IsRegistrationOpen)
        {
            TempData["Error"] = "Реєстрація закрита.";
            return RedirectToAction(nameof(Details), new { id });
        }

        if (ev.FreePlaces <= 0)
        {
            TempData["Error"] = "Немає вільних місць.";
            return RedirectToAction(nameof(Details), new { id });
        }

        var existing = ev.Registrations.FirstOrDefault(r => r.StudentId == userId);

        if (existing != null)
        {
            TempData["Info"] = "Ви вже подавали заявку.";
            return RedirectToAction(nameof(Details), new { id });
        }

        _db.EventRegistrations.Add(new EventRegistration
        {
            EduEventId = id,
            StudentId = userId,
            Status = RegistrationStatus.Pending,
            RegisteredAt = DateTime.Now
        });

        await _db.SaveChangesAsync();

        TempData["Success"] = "Запит на реєстрацію надіслано.";

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [Authorize(Roles = "Teacher,Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Confirm(int registrationId, int eventId)
    {
        var reg = await _db.EventRegistrations
            .Include(r => r.EduEvent)
            .ThenInclude(e => e!.Registrations)
            .FirstOrDefaultAsync(r => r.Id == registrationId);

        if (reg == null)
            return NotFound();

        if (reg.EduEvent!.FreePlaces <= 0 && reg.Status != RegistrationStatus.Confirmed)
        {
            TempData["Error"] = "Немає місць.";
            return RedirectToAction(nameof(Details), new { id = eventId });
        }

        reg.Status = RegistrationStatus.Confirmed;

        await _db.SaveChangesAsync();

        TempData["Success"] = "Підтверджено.";

        return RedirectToAction(nameof(Details), new { id = eventId });
    }

    [HttpPost]
    [Authorize(Roles = "Teacher,Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(int registrationId, int eventId)
    {
        var reg = await _db.EventRegistrations.FindAsync(registrationId);

        if (reg == null)
            return NotFound();

        reg.Status = RegistrationStatus.Rejected;

        await _db.SaveChangesAsync();

        TempData["Success"] = "Відхилено.";

        return RedirectToAction(nameof(Details), new { id = eventId });
    }
}