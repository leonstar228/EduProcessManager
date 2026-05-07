using EduProcessManager.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduProcessManager.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ApplicationDbContext _db;
    public HomeController(ApplicationDbContext db) => _db = db;
    public async Task<IActionResult> Index()
    {
        ViewBag.Tests = await _db.Tests.CountAsync();
        ViewBag.Events = await _db.EduEvents.CountAsync();
        ViewBag.Attempts = await _db.TestAttempts.CountAsync();
        ViewBag.Registrations = await _db.EventRegistrations.CountAsync();
        return View();
    }
}
