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
            if (await this.userManager.FindByEmailAsync(registerUser.Email) is not null)
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

    [HttpPost]
    public async Task<IActionResult> VerifyEmail(CheckEmailViewModel verifyEmailModel)
    {
        if (this.ModelState.IsValid && verifyEmailModel is not null)
        {
            IdentityUser user = await this.userManager.FindByEmailAsync(verifyEmailModel.Email);

            if (user is not null)
            {
                var token = this.userManager.GenerateEmailConfirmationTokenAsync(user);
            }
        }

        return this.View();
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult ForgotPassword()
    {
        string url = $"{this.Request.Scheme}://{this.Request.Host}/Account/ChangePassword";
        return this.View(new CheckEmailViewModel { ClientUri = url });
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
                var queryParameters = new Dictionary<string, string>()
                {
                    { "token", token },
                    { "email", checkEmailModel.Email },
                };

                var resetPasswordUrl = QueryHelpers.AddQueryString(checkEmailModel.ClientUri, queryParameters!);
                await this.emailSender.SendEmailAsync(checkEmailModel.Email, "Reset Password", $"Hi, {user.UserName}!\nYou requested to reset your password. Please click the link below to reset it:\n{resetPasswordUrl}");
                return this.RedirectToAction("Message", new { message = $"Message was succesfuly sent on email {checkEmailModel.Email}! You can use generated link during 2 hours." });
            }
        }

        return this.View();
    }

    [AllowAnonymous]
    public IActionResult Message(string message)
    {
        if (string.IsNullOrEmpty(message))
        {
            return this.View("Error", new ErrorViewModel { RequestId = "Invalid Model State" });
        }

        return this.View("Message", message);
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult ChangePassword(string token, string email)
    {
        if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
        {
            return this.BadRequest("Invalid token or email.");
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
}
