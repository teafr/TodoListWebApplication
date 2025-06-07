using System.ComponentModel.DataAnnotations;

namespace TodoListApp.WebApp.Models.ViewModels;

public class ChangePasswordViewModel
{
    [Required(ErrorMessage = "Pur your email"), EmailAddress]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [StringLength(40, MinimumLength = 8, ErrorMessage = "The {0} must be at least {2} and at max {1} character long.")]
    [DataType(DataType.Password)]
    [Compare("ConfirmNewPassword", ErrorMessage = "The password and confirmation password do not match.")]
    [Display(Name = "New Password")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirm Password is required")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm New Password")]
    public string ConfirmNewPassword { get; set; } = string.Empty;
}
