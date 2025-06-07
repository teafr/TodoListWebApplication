using Microsoft.AspNetCore.Identity;

namespace TodoListApp.WebApp.Helpers;
public interface ITokenGenerator
{
    string GenerateToken(IdentityUser user);
}