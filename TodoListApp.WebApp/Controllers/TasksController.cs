using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApp.Extensions;
using TodoListApp.WebApp.Helpers;
using TodoListApp.WebApp.Models;
using TodoListApp.WebApp.Models.ViewModels;
using TodoListApp.WebApp.Models.ViewModels.AuthenticationModels;
using TodoListApp.WebApp.Services;

namespace TodoListApp.WebApp.Controllers;

[Authorize]
public class TasksController : Controller
{
    private readonly ITaskWebApiService apiService;
    private readonly UserManager<ApplicationUser> userManager;

    public TasksController(ITaskWebApiService apiService, UserManager<ApplicationUser> userManager)
    {
        this.apiService = apiService;
        this.userManager = userManager;
    }

    [AllowAnonymous]
    public async Task<IActionResult> Index([FromQuery] TaskFilterModel filter)
    {
        if (this.ModelState.IsValid && filter is not null)
        {
            var userId = this.userManager.GetUserId(this.User);
            if (userId is null)
            {
                return this.View(new ListOfTasks(new List<TaskModel>(), new TaskFilterModel()));
            }

            List<TaskModel>? tasks = await this.apiService.GetTasksByUserIdAsync(userId);

            if (filter.StatusId == 0)
            {
                if (!filter.ShowCompletedTasks)
                {
                    tasks = tasks?.Where(task => task.Status.Id != (int)StatusOfTask.Completed).ToList();
                }
            }
            else
            {
                tasks = tasks?.Where(task => task.Status.Id == filter.StatusId).ToList();
            }

            if (!string.IsNullOrEmpty(filter.Tag))
            {
                tasks = tasks?.Where(task => task.Tags != null && task.Tags.Contains(filter.Tag)).ToList();
            }

            tasks = filter.SortBy switch
            {
                "Title" => tasks?.OrderBy(t => t.Title).ToList(),
                "CreationDate" => tasks?.OrderBy(t => t.CreationDate).ToList(),
                "DueDate" => tasks?.OrderBy(t => t.DueDate).ToList(),
                _ => tasks
            };

            return this.View(new ListOfTasks(tasks ?? new List<TaskModel>(), filter, filter.CurrentPage));
        }

        return this.View("Error", new ErrorViewModel { RequestId = "Invalid Model State" });
    }

    public async Task<IActionResult> Details(int taskId)
    {
        if (this.ModelState.IsValid)
        {
            TaskModel? task = await this.apiService.GetTaskByIdAsync(taskId);
            if (task is null)
            {
                return this.NotFound();
            }

            return this.View(task.ToTaskViewModel(await this.userManager.FindByIdAsync(task.AssigneeId)));
        }

        return this.View("Error", new ErrorViewModel { RequestId = "Invalid Model State" });
    }

    public async Task<IActionResult> GetTags()
    {
        List<string>? tags = await this.apiService.GetTagsByUserIdAsync(this.userManager.GetUserId(this.User));
        return this.View(tags ?? new List<string>());
    }

    public async Task<IActionResult> SearchTasks(string? query, string? property)
    {
        if (this.ModelState.IsValid)
        {
            if (property is null || query is null)
            {
                return this.View(new List<TaskViewModel>());
            }

            List<TaskModel>? tasks = await this.apiService.GetTasksByUserIdAsync(this.userManager.GetUserId(this.User));
            tasks = property switch
            {
                "Title" => tasks?.Where(task => task.Title.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList(),
                "DueDate" => tasks?.Where(task => task.DueDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture) == query).ToList(),
                "CreationDate" => tasks?.Where(task => task.CreationDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture) == query).ToList(),
                _ => tasks,
            };

            return this.View(tasks?.Select(task => task.ToTaskViewModel()).ToList() ?? new List<TaskViewModel>());
        }

        return this.View("Error", new ErrorViewModel { RequestId = "Invalid Model State" });
    }

    public IActionResult AddTask(int todoListId)
    {
        if (this.ModelState.IsValid)
        {
            return this.View(new TaskViewModel() { TodoListId = todoListId });
        }

        return this.View("Error", new ErrorViewModel { RequestId = "Invalid Model State" });
    }

    [HttpPost]
    public async Task<IActionResult> AddTask(TaskViewModel taskViewModel)
    {
        ExceptionHelper.CheckObjectForNull(taskViewModel);

        if (this.ModelState.IsValid)
        {
            taskViewModel.Assignee = await this.userManager.GetUserAsync(this.User);
            taskViewModel.Status = new StatusViewModel() { Id = (int)StatusOfTask.NotStarted, Name = string.Empty };
            taskViewModel.CreationDate = DateTime.Now;

            await this.apiService.CreateTaskAsync(new TaskModel(taskViewModel));
            return this.RedirectToAction("GetTasks", "TodoLists", new { todoListId = taskViewModel.TodoListId });
        }

        return this.View(new TaskViewModel() { TodoListId = taskViewModel.TodoListId });
    }

    [HttpPost]
    public async Task<IActionResult> AddComment(int taskId, string comment)
    {
        if (this.ModelState.IsValid)
        {
            TaskModel? task = await this.apiService.GetTaskByIdAsync(taskId);
            if (task is null)
            {
                return this.NotFound();
            }

            if (task.Comments?.Contains(comment) ?? false)
            {
                return this.View("Error", new ErrorViewModel { RequestId = "Comment already exists" });
            }

            await this.apiService.AddCommentAsync(taskId, comment);
            return this.RedirectToAction("Details", new { taskId });
        }

        return this.View("Error", new ErrorViewModel { RequestId = "Invalid Model State" });
    }

    [HttpPost]
    public async Task<IActionResult> AddTag(int taskId, string tag)
    {
        if (this.ModelState.IsValid)
        {
            TaskModel? task = await this.apiService.GetTaskByIdAsync(taskId);
            if (task is null)
            {
                return this.NotFound();
            }

            if (task.Tags?.Contains(tag) ?? false)
            {
                return this.View("Error", new ErrorViewModel { RequestId = "Tag already exists" });
            }

            await this.apiService.AddTagAsync(taskId, tag);
            return this.RedirectToAction("Details", new { taskId });
        }

        return this.View("Error", new ErrorViewModel { RequestId = "Invalid Model State" });
    }

    public async Task<IActionResult> Edit(int taskId)
    {
        if (this.ModelState.IsValid)
        {
            var task = await this.apiService.GetTaskByIdAsync(taskId);
            if (task is null)
            {
                return this.NotFound();
            }

            return this.View(task.ToTaskViewModel(await this.userManager.FindByIdAsync(task.AssigneeId)) ?? new TaskViewModel());
        }

        return this.View("Error", new ErrorViewModel { RequestId = "Invalid Model State" });
    }

    [HttpPost]
    public async Task<IActionResult> Edit(TaskViewModel taskViewModel)
    {
        if (this.ModelState.IsValid)
        {
            await this.apiService.UpdateTaskAsync(new TaskModel(taskViewModel));
            return this.RedirectToAction("GetTasks", "TodoLists", new { todoListId = taskViewModel.TodoListId });
        }

        return this.View("Error", new ErrorViewModel { RequestId = "Invalid Model State" });
    }

    [HttpPost]
    public async Task<IActionResult> EditComment(int taskId, string oldComment, string newComment)
    {
        if (!this.ModelState.IsValid)
        {
            return this.View("Error", new ErrorViewModel { RequestId = "Invalid Model State" });
        }

        if (string.IsNullOrWhiteSpace(newComment))
        {
            this.ModelState.AddModelError("newComment", "Comment cannot be empty.");
        }

        TaskModel? task = await this.apiService.GetTaskByIdAsync(taskId);
        if (task is null || !(task.Comments?.Contains(oldComment) ?? false))
        {
            return this.NotFound();
        }

        if (task.Comments.Contains(newComment))
        {
            this.ModelState.AddModelError("newComment", "This comment already exists.");
        }

        if (string.IsNullOrEmpty(newComment))
        {
            return this.View("Error", new ErrorViewModel { RequestId = "Comment can't be empty" });
        }

        await this.apiService.UpdateCommentInTaskAsync(taskId, oldComment, newComment);
        return this.RedirectToAction("Details", new { taskId });
    }

    public async Task<IActionResult> RemoveComment(int taskId, string comment)
    {
        if (this.ModelState.IsValid)
        {
            TaskModel? task = await this.apiService.GetTaskByIdAsync(taskId);
            if (task is null || !(task.Comments?.Contains(comment) ?? false))
            {
                return this.NotFound();
            }

            await this.apiService.RemoveCommentFromTaskAsync(taskId, comment);
            return this.RedirectToAction("Details", new { taskId });
        }

        return this.View("Error", new ErrorViewModel { RequestId = "Invalid Model State" });
    }

    public async Task<IActionResult> RemoveTag(int taskId, string tag)
    {
        if (this.ModelState.IsValid)
        {
            TaskModel? task = await this.apiService.GetTaskByIdAsync(taskId);
            if (task is null || !(task.Tags?.Contains(tag) ?? false))
            {
                return this.NotFound();
            }

            await this.apiService.RemoveTagFromTaskAsync(taskId, tag);
            return this.RedirectToAction("Details", new { taskId });
        }

        return this.View("Error", new ErrorViewModel { RequestId = "Invalid Model State" });
    }

    public async Task<IActionResult> AssignTask(int taskId, string assigneeId)
    {
        if (this.ModelState.IsValid)
        {
            TaskModel? task = await this.apiService.GetTaskByIdAsync(taskId);
            if (task is null)
            {
                return this.NotFound();
            }

            ApplicationUser identityUser = await this.userManager.FindByIdAsync(assigneeId);
            if (identityUser is null)
            {
                return this.NotFound();
            }

            await this.apiService.UpdateAssigneeAsync(taskId, assigneeId);
            return this.RedirectToAction("Details", new { taskId });
        }

        return this.View("Error", new ErrorViewModel { RequestId = "Invalid Model State" });
    }

    public async Task<IActionResult> ChangeStatus(int taskId, int statusId)
    {
        if (this.ModelState.IsValid)
        {
            TaskModel? task = await this.apiService.GetTaskByIdAsync(taskId);
            if (task is null)
            {
                return this.NotFound();
            }

            await this.apiService.UpdateStatusOfTaskAsync(taskId, statusId);

            return this.RedirectToAction("Details", new { taskId });
        }

        return this.View("Error", new ErrorViewModel { RequestId = "Invalid Model State" });
    }

    public async Task<IActionResult> Delete(int taskId)
    {
        if (this.ModelState.IsValid)
        {
            TaskModel? task = await this.apiService.GetTaskByIdAsync(taskId);
            if (task is null)
            {
                return this.NotFound();
            }

            await this.apiService.DeleteTaskAsync(taskId);
            return this.RedirectToAction("GetTasks", "TodoLists", new { todoListId = task.TodoListId });
        }

        return this.View("Error", new ErrorViewModel { RequestId = "Invalid Model State" });
    }
}
