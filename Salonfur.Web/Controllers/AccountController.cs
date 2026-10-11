using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Salonfur.Core.Entities;
using Salonfur.Web.ViewModels.Account;

namespace Salonfur.Web.Controllers;

[Route("admin")]
public class AccountController(SignInManager<AppUser> signInManager) : Controller
{
    [HttpGet("login")]
    public IActionResult Login(string? returnUrl)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Admin");

        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost("login")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: true);
        if (result.Succeeded)
            return Url.IsLocalUrl(model.ReturnUrl) ? LocalRedirect(model.ReturnUrl) : RedirectToAction("Index", "Admin");

        ModelState.AddModelError(string.Empty, result.IsLockedOut
            ? "Te veel mislukte pogingen. Probeer het over 15 minuten opnieuw."
            : "E-mail of wachtwoord klopt niet.");
        model.Password = "";
        return View(model);
    }

    [HttpPost("logout")]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        return Redirect("/");
    }
}
