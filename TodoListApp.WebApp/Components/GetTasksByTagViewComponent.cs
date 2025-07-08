using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.ApiClient.Services;
using TodoListApp.WebApp.Extensions;
using TodoListApp.WebApp.Models;
using TodoListApp.WebApp.Models.AuthenticationModels;

namespace TodoListApp.WebApp.Components;

[Authorize]
public class GetTasksByTagViewComponent : ViewComponent
{
    private readonly ITaskApiClientService apiService;
    private readonly UserManager<ApplicationUser> userManager;

    public GetTasksByTagViewComponent(ITaskApiClientService apiService, UserManager<ApplicationUser> userManager)
    {
        this.apiService = apiService;
        this.userManager = userManager;
    }

    public async Task<IViewComponentResult> InvokeAsync(string tag)
    {
        if (this.ModelState.IsValid)
        {
            var tasks = await this.apiService.GetTasksByUserIdAsync(this.userManager.GetUserId(this.UserClaimsPrincipal));
            return this.View(tasks?.Where(task => task.Tags != null && task.Tags.Contains(tag))?.Select(taskModel => taskModel.ToTaskViewModel())?.ToList() ?? new List<TaskViewModel>());
        }

        return this.View("Error", new ErrorViewModel { RequestId = "Invalid Model State" });
    }
}
