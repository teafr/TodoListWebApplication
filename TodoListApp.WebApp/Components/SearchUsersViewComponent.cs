using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoListApp.WebApp.Models.ViewModels;

namespace TodoListApp.WebApp.Components;

[Authorize]
public class SearchUsersViewComponent : ViewComponent
{
    private readonly UserManager<IdentityUser> userManager;

    public SearchUsersViewComponent(UserManager<IdentityUser> userManager)
    {
        this.userManager = userManager;
    }

    public async Task<IViewComponentResult> InvokeAsync(int taskId, string searchQuery)
    {
        if (this.ModelState.IsValid)
        {
            List<IdentityUser> users = await this.userManager.Users.Where(u => EF.Functions.Like(u.UserName, $"%{searchQuery.ToUpperInvariant()}%")).ToListAsync();
            return this.View((users, taskId));
        }

        return this.View("Error", new ErrorViewModel { RequestId = "Invalid Model State" });
    }
}
