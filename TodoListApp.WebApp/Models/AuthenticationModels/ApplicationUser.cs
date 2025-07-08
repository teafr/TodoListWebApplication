using Microsoft.AspNetCore.Identity;

namespace TodoListApp.WebApp.Models.AuthenticationModels;

public class ApplicationUser : IdentityUser
{
    public string HasAccsses { get; set; } = "[]";
}
