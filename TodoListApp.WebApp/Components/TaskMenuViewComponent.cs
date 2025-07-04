using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApp.Models.ViewModels.AuthenticationModels;
using TodoListApp.WebApp.Services;

namespace TodoListApp.WebApp.Components;

public class TaskMenuViewComponent : ViewComponent
{
    private readonly ITaskWebApiService taskListWebApiService;
    private readonly UserManager<ApplicationUser> userManager;

    public TaskMenuViewComponent(ITaskWebApiService taskListWebApiService, UserManager<ApplicationUser> userManager)
    {
        this.taskListWebApiService = taskListWebApiService;
        this.userManager = userManager;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        string userId = this.userManager.GetUserId(this.UserClaimsPrincipal);
        if (userId is null)
        {
            return this.View(new List<string>());
        }

        return this.View(await this.taskListWebApiService.GetTagsByUserIdAsync(userId));
    }
}
