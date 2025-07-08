using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.ApiClient.Services;
using TodoListApp.WebApp.Models.AuthenticationModels;
using TodoListApp.WebApp.Services;

namespace TodoListApp.WebApp.Components;

public class TaskMenuViewComponent : ViewComponent
{
    private readonly ITaskApiClientService taskListWebApiService;
    private readonly UserManager<ApplicationUser> userManager;

    public TaskMenuViewComponent(ITaskApiClientService taskListWebApiService, UserManager<ApplicationUser> userManager)
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
