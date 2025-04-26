using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApp.Helpers;
using TodoListApp.WebApp.Models.ViewModels;

namespace TodoListApp.WebApp.Controllers;

[Authorize]
public class AccountController : Controller
{
    private readonly UserManager<IdentityUser> userManager;
    private readonly SignInManager<IdentityUser> signInManager;
    private readonly JwtTokenGenerator jwtTokenGenerator;

    public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, JwtTokenGenerator jwtTokenGenerator)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.jwtTokenGenerator = jwtTokenGenerator;
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
            ExceptionHelper.CheckViewModel(loginUser);
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
        ExceptionHelper.CheckViewModel(registerUser);

        if (this.ModelState.IsValid)
        {
            IdentityUser user = new IdentityUser
            {
                UserName = registerUser.Username,
                Email = registerUser.Email,
            };

            var result = await this.userManager.CreateAsync(user, registerUser.Password);
            if (result.Succeeded)
            {
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
