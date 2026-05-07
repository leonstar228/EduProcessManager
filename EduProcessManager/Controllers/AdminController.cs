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
    private readonly RoleManager<IdentityRole> _roleManager;

    public AdminController(ApplicationDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _db = db;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<IActionResult> Users()
    {
        var allUsers = await _userManager.Users.OrderBy(u => u.FullName).ToListAsync();
        var result = new List<UserListItemViewModel>();

        foreach (var user in allUsers)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "";

            if (User.IsInRole("SuperAdmin") && role != "Admin") continue;
            if (User.IsInRole("Admin") && role != "Teacher" && role != "Student") continue;

            result.Add(new UserListItemViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? "",
                IsActive = user.IsActive,
                Role = role
            });
        }

        return View(result);
    }

    public IActionResult CreateUser()
    {
        ViewBag.AllowedRoles = GetAllowedRoles();
        return View(new UserFormViewModel { IsActive = true });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateUser(UserFormViewModel model)
    {
        var allowedRoles = GetAllowedRoles();
        ViewBag.AllowedRoles = allowedRoles;

        if (!allowedRoles.Contains(model.Role))
            ModelState.AddModelError(nameof(model.Role), "Вибрана роль недоступна для вашого облікового запису.");

        if (!ModelState.IsValid) return View(model);

        if (!await _roleManager.RoleExistsAsync(model.Role))
            await _roleManager.CreateAsync(new IdentityRole(model.Role));

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            FullName = model.FullName,
            IsActive = model.IsActive,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, model.Role);
            TempData["Success"] = "Користувача успішно створено.";
            return RedirectToAction(nameof(Users));
        }

        foreach (var error in result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);

        return View(model);
    }

    public async Task<IActionResult> EditUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? "";
        if (!CanManageRole(role)) return Forbid();

        ViewBag.AllowedRoles = GetAllowedRoles();
        return View(new UserFormViewModel
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email ?? "",
            Role = role,
            IsActive = user.IsActive
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditUser(UserFormViewModel model)
    {
        var allowedRoles = GetAllowedRoles();
        ViewBag.AllowedRoles = allowedRoles;

        if (!allowedRoles.Contains(model.Role))
            ModelState.AddModelError(nameof(model.Role), "Вибрана роль недоступна для вашого облікового запису.");

        var user = await _userManager.FindByIdAsync(model.Id ?? "");
        if (user == null) return NotFound();

        var oldRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? "";
        if (!CanManageRole(oldRole)) return Forbid();

        ModelState.Remove(nameof(model.Password));
        if (!ModelState.IsValid) return View(model);

        user.FullName = model.FullName;
        user.Email = model.Email;
        user.UserName = model.Email;
        user.IsActive = model.IsActive;

        await _userManager.UpdateAsync(user);

        if (oldRole != model.Role)
        {
            if (!string.IsNullOrWhiteSpace(oldRole)) await _userManager.RemoveFromRoleAsync(user, oldRole);
            await _userManager.AddToRoleAsync(user, model.Role);
        }

        if (!string.IsNullOrWhiteSpace(model.Password))
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            await _userManager.ResetPasswordAsync(user, token, model.Password);
        }

        TempData["Success"] = "Дані користувача оновлено.";
        return RedirectToAction(nameof(Users));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? "";
        if (!CanManageRole(role)) return Forbid();

        await _userManager.DeleteAsync(user);
        TempData["Success"] = "Користувача видалено.";
        return RedirectToAction(nameof(Users));
    }

    public async Task<IActionResult> Logs()
    {
        var logs = await _db.AuditLogs.OrderByDescending(x => x.CreatedAt).Take(100).ToListAsync();
        return View(logs);
    }

    private string[] GetAllowedRoles()
    {
        if (User.IsInRole("SuperAdmin")) return new[] { "Admin" };
        return new[] { "Teacher", "Student" };
    }

    private bool CanManageRole(string role)
    {
        if (User.IsInRole("SuperAdmin")) return role == "Admin";
        if (User.IsInRole("Admin")) return role == "Teacher" || role == "Student";
        return false;
    }
}

public class UserListItemViewModel
{
    public string Id { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class UserFormViewModel
{
    public string? Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
