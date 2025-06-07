using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApp.Helpers;
using TodoListApp.WebApp.Models.ViewModels;

namespace TodoListApp.WebApp.Controllers;

[Authorize]
public class AccountController : Controller
{
    private readonly UserManager<IdentityUser> userManager;
    private readonly SignInManager<IdentityUser> signInManager;
    private readonly ITokenGenerator jwtTokenGenerator;
    private readonly IEmailSender emailSender;

    public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ITokenGenerator jwtTokenGenerator, IEmailSender emailSender)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.jwtTokenGenerator = jwtTokenGenerator;
        this.emailSender = emailSender;
    }

    [AllowAnonymous]
    public ViewResult Login()
    {
        return this.View();
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel loginUser)
    {
        if (this.ModelState.IsValid)
        {
            ExceptionHelper.CheckObjectForNull(loginUser);
            IdentityUser user = await this.userManager.FindByNameAsync(loginUser.Username);

            if (user != null)
            {
                var result = await this.signInManager.PasswordSignInAsync(loginUser.Username, loginUser.Password, loginUser.RememberMe, false);
                if (result.Succeeded)
                {
                    this.Response.Cookies.Append("JWToken", this.jwtTokenGenerator.GenerateToken(user), new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Lax,
                        Expires = loginUser.RememberMe ? DateTimeOffset.UtcNow.AddDays(7) : DateTimeOffset.UtcNow.AddHours(1),
                    });
                    await this.emailSender.SendEmailAsync(user.Email, "Login", $"Hi, {user.UserName}! You successfully logged in.");

                    return this.Redirect("/");
                }
                else
                {
                    this.ModelState.AddModelError(string.Empty, "Login isn't successful.");
                }
            }
            else
            {
                this.ModelState.AddModelError(string.Empty, "User with such name doesn't exist.");
            }
        }

        return this.View(loginUser);
    }

    public async Task<IActionResult> Logout()
    {
        await this.signInManager.SignOutAsync();
        this.Response.Cookies.Delete("JWToken");
        return this.Redirect("/");
    }

    [AllowAnonymous]
    public ViewResult Register()
    {
        return this.View();
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel registerUser)
    {
        ExceptionHelper.CheckObjectForNull(registerUser);

        if (this.ModelState.IsValid)
        {
            if (this.userManager.FindByEmailAsync(registerUser.Email) is not null)
            {
                this.ModelState.AddModelError(string.Empty, "User with such email already exists.");
                return this.View(registerUser);
            }

            IdentityUser user = new IdentityUser
            {
                UserName = registerUser.Username,
                Email = registerUser.Email,
            };

            var result = await this.userManager.CreateAsync(user, registerUser.Password);
            if (result.Succeeded)
            {
                await this.emailSender.SendEmailAsync(user.Email, "Registration", $"Hi, {user.UserName}!\nYou successfully registered in To-do List web application.");
                return this.RedirectToAction("Login");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    this.ModelState.AddModelError(string.Empty, error.Description);
                }
            }
        }

        return this.View(registerUser);
    }
}
