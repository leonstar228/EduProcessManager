using EduProcessManager.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using EduProcessManager.Models;

namespace EduProcessManager.Controllers;

public class AccountController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    public AccountController(SignInManager<ApplicationUser> signInManager) => _signInManager = signInManager;

    [AllowAnonymous]
    public IActionResult Login() => View(new LoginViewModel());

    [HttpPost, AllowAnonymous]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
        if (result.Succeeded) return RedirectToAction("Index", "Home");
        ModelState.AddModelError(string.Empty, "Неправильний email або пароль");
        return View(model);
    }

    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login");
    }

    public IActionResult AccessDenied() => View();
}
