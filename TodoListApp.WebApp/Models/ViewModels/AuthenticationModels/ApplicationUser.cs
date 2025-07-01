using Microsoft.AspNetCore.Identity;

namespace TodoListApp.WebApp.Models.ViewModels.AuthenticationModels;

public class ApplicationUser : IdentityUser
{
    public string HasAccsses { get; init; } = string.Empty;
}
