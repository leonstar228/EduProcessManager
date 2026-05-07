using EduProcessManager.Data;
using EduProcessManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduProcessManager.Controllers;

[Authorize(Roles = "Admin,SuperAdmin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    public AdminController(ApplicationDbContext db, UserManager<ApplicationUser> userManager) { _db = db; _userManager = userManager; }
    public async Task<IActionResult> Users()
    {
        var users = await _userManager.Users.OrderBy(u => u.FullName).ToListAsync();
        return View(users);
    }
    [HttpPost]
    public async Task<IActionResult> ToggleUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user != null) { user.IsActive = !user.IsActive; await _userManager.UpdateAsync(user); }
        return RedirectToAction(nameof(Users));
    }
    public async Task<IActionResult> Logs()
    {
        var logs = await _db.AuditLogs.OrderByDescending(x => x.CreatedAt).Take(100).ToListAsync();
        return View(logs);
    }
}
