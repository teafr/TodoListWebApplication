using System.ComponentModel.DataAnnotations;

namespace TodoListApp.WebApp.Models.ViewModels.AuthenticationModels;

public class CheckEmailViewModel
{
    [Required(ErrorMessage = "Pur your email"), EmailAddress]
    public string Email { get; set; } = string.Empty;
}
