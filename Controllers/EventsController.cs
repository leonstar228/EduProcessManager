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
    public EventsController(ApplicationDbContext db, UserManager<ApplicationUser> userManager) { _db = db; _userManager = userManager; }

    public async Task<IActionResult> Index()
    {
        var items = await _db.EduEvents.Include(e => e.Registrations).OrderBy(e => e.StartAt).ToListAsync();
        return View(items);
    }

    [Authorize(Roles = "Teacher,Admin,SuperAdmin")]
    public IActionResult Create() => View(new EduEvent { StartAt = DateTime.Now.AddDays(1), Capacity = 25 });

    [HttpPost, Authorize(Roles = "Teacher,Admin,SuperAdmin")]
    public async Task<IActionResult> Create(EduEvent model)
    {
        model.CreatedById = _userManager.GetUserId(User)!;
        if (!ModelState.IsValid) return View(model);
        _db.EduEvents.Add(model);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int id)
    {
        var item = await _db.EduEvents.Include(e => e.Registrations).ThenInclude(r => r.Student).FirstOrDefaultAsync(e => e.Id == id);
        if (item == null) return NotFound();
        return View(item);
    }

    [HttpPost, Authorize(Roles = "Student")]
    public async Task<IActionResult> Register(int id)
    {
        var userId = _userManager.GetUserId(User)!;
        var ev = await _db.EduEvents.Include(e => e.Registrations).FirstOrDefaultAsync(e => e.Id == id);
        if (ev == null || !ev.IsRegistrationOpen || ev.FreePlaces <= 0) return RedirectToAction(nameof(Index));
        if (!ev.Registrations.Any(r => r.StudentId == userId))
            _db.EventRegistrations.Add(new EventRegistration { EduEventId = id, StudentId = userId, Status = RegistrationStatus.Pending });
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost, Authorize(Roles = "Teacher,Admin,SuperAdmin")]
    public async Task<IActionResult> Confirm(int registrationId, int eventId)
    {
        var reg = await _db.EventRegistrations.FindAsync(registrationId);
        if (reg != null) { reg.Status = RegistrationStatus.Confirmed; await _db.SaveChangesAsync(); }
        return RedirectToAction(nameof(Details), new { id = eventId });
    }
}
