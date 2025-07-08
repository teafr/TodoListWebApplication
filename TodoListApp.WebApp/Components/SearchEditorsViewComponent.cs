using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoListApp.WebApp.Models;
using TodoListApp.WebApp.Models.AuthenticationModels;

namespace TodoListApp.WebApp.Components;

[Authorize]
public class SearchEditorsViewComponent : ViewComponent
{
    private readonly UserManager<ApplicationUser> userManager;

    public SearchEditorsViewComponent(UserManager<ApplicationUser> userManager)
    {
        this.userManager = userManager;
    }

    public async Task<IViewComponentResult> InvokeAsync(int todoListId, string searchQuery)
    {
        if (this.ModelState.IsValid)
        {
            List<ApplicationUser> users = await this.userManager.Users.Where(u => EF.Functions.Like(u.UserName, $"%{searchQuery.ToUpperInvariant()}%")).ToListAsync();
            return this.View((users, todoListId));
        }

        return this.View("Error", new ErrorViewModel { RequestId = "Invalid Model State" });
    }
}
