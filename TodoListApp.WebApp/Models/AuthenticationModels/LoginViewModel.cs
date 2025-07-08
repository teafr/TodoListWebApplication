using System.ComponentModel.DataAnnotations;

namespace TodoListApp.WebApp.Models.AuthenticationModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "User is required.")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Remember me?")]
    public bool RememberMe { get; set; }
}
