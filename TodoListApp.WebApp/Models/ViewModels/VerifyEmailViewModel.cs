using System.ComponentModel.DataAnnotations;

namespace TodoListApp.WebApp.Models.ViewModels;

public class VerifyEmailViewModel
{
    [Required(ErrorMessage = "Pur your email"), EmailAddress]
    public string Email { get; set; } = string.Empty;
}
