using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
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
            if (await this.userManager.FindByEmailAsync(registerUser.Email) is null)
            {
                IdentityUser user = new IdentityUser
                {
                    UserName = registerUser.Username,
                    Email = registerUser.Email,
                };

                var result = await this.userManager.CreateAsync(user, registerUser.Password);
                if (result.Succeeded)
                {
                    string token = await this.userManager.GenerateEmailConfirmationTokenAsync(user);
                    await this.emailSender.SendEmailAsync(user.Email, "Registration", $"Hi, {user.UserName}!\nYou successfully registered in To-do List web application. Verify your email by link: {this.GetUri(token, registerUser.Email, action: "VerifyEmail")}");
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
            else
            {
                this.ModelState.AddModelError(string.Empty, "User with such email already exists.");
                return this.View(registerUser);
            }
        }

        return this.View(registerUser);
    }

    [AllowAnonymous]
    public async Task<IActionResult> VerifyEmail(string token, string email)
    {
        if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(email))
        {
            IdentityUser user = await this.userManager.FindByEmailAsync(email);

            if (user is not null)
            {
                var result = await this.userManager.ConfirmEmailAsync(user, token);

                if (result.Succeeded)
                {
                    return this.RedirectToAction("Message", new { message = $"Email {email} was succesfuly verified!" });
                }
            }
        }

        return this.View("Error", new ErrorViewModel { RequestId = $"Email or token isn't valid, so email wasn't verified." });
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult ForgotPassword()
    {
        return this.View();
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> ForgotPassword(CheckEmailViewModel checkEmailModel)
    {
        if (this.ModelState.IsValid)
        {
            ExceptionHelper.CheckObjectForNull(checkEmailModel);
            IdentityUser user = await this.userManager.FindByEmailAsync(checkEmailModel.Email);

            if (user is not null)
            {
                var token = await this.userManager.GeneratePasswordResetTokenAsync(user);
                await this.emailSender.SendEmailAsync(user.Email, "Reset Password", $"Hi, {user.UserName}!\nYou requested to reset your password. Please click the link below to reset it:\n{this.GetUri(token, user.Email, action: "ChangePassword")}");
                return this.RedirectToAction("Message", new { message = $"Message was succesfuly sent on email {user.Email}! You can use generated link during 2 hours." });
            }
        }

        return this.View();
    }

    [AllowAnonymous]
    public IActionResult Message(string message)
    {
        if (!string.IsNullOrEmpty(message))
        {
            return this.View("Message", message);
        }

        return this.View("Error", new ErrorViewModel { RequestId = "Invalid Model State" });
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult ChangePassword(string token, string email)
    {
        if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
        {
            return this.View("Error", new ErrorViewModel { RequestId = "Invalid Model State" });
        }

        return this.View(new ChangePasswordViewModel
        {
            Token = token,
            Email = email,
        });
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel changePasswordModel)
    {
        if (this.ModelState.IsValid && changePasswordModel is not null)
        {
            IdentityUser user = await this.userManager.FindByEmailAsync(changePasswordModel.Email);
            if (user is not null)
            {
                var result = await this.userManager.ResetPasswordAsync(user, changePasswordModel.Token, changePasswordModel.NewPassword);
                if (result.Succeeded)
                {
                    await this.emailSender.SendEmailAsync(user.Email, "Change Password", $"Hi, {user.UserName}!\nYou successfully changed your password.");
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
            else
            {
                this.ModelState.AddModelError(string.Empty, "User with such email doesn't exist.");
            }
        }

        return this.View(changePasswordModel);
    }

    public Uri GetUri(string token, string email, string action)
    {
        var queryParameters = new Dictionary<string, string>()
        {
            { "token", token },
            { "email",  email },
        };

        return new Uri(QueryHelpers.AddQueryString($"{this.Request.Scheme}://{this.Request.Host}/Account/{action}", queryParameters!));
    }
}
