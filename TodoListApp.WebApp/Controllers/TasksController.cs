using System.Globalization;
using System.Net;
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

            List<TaskModel>? tasks;

            try
            {
                tasks = await this.apiService.GetTasksByUserIdAsync(userId);
            }
            catch (HttpRequestException)
            {
                return this.RedirectToAction("Login", "Account");
            }

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

            var result = await this.apiService.CreateTaskAsync(new TaskModel(taskViewModel));
            if (result.StatusCode == HttpStatusCode.Created)
            {
                return this.RedirectToAction("GetTasks", "TodoLists", new { todoListId = taskViewModel.TodoListId });
            }
            else if (result.StatusCode == HttpStatusCode.BadRequest)
            {
                this.ModelState.AddModelError(string.Empty, "Invalid task");
            }

            throw new InvalidOperationException("Failed to create a new task.");
        }

        return this.View(new TaskViewModel() { TodoListId = taskViewModel.TodoListId });
    }

    [HttpPost]
    public async Task<IActionResult> AddComment(int taskId, string comment)
    {
        if (this.ModelState.IsValid)
        {
            var result = await this.apiService.AddCommentAsync(taskId, comment);
            if (result.StatusCode == HttpStatusCode.NoContent)
            {
                return this.RedirectToAction("Details", new { taskId });
            }
            else if (result.StatusCode == HttpStatusCode.NotFound)
            {
                return this.NotFound();
            }
            else if (result.StatusCode == HttpStatusCode.BadRequest)
            {
                return this.View("Error", new ErrorViewModel { RequestId = "Comment invalid or already exists" });
            }

            throw new InvalidOperationException("Failed to add comment to the task.");
        }

        return this.View("Error", new ErrorViewModel { RequestId = "Invalid Model State" });
    }

    [HttpPost]
    public async Task<IActionResult> AddTag(int taskId, string tag)
    {
        if (this.ModelState.IsValid)
        {
            var result = await this.apiService.AddTagAsync(taskId, tag);
            if (result.StatusCode == HttpStatusCode.NoContent)
            {
                return this.RedirectToAction("Details", new { taskId });
            }
            else if (result.StatusCode == HttpStatusCode.NotFound)
            {
                return this.NotFound();
            }
            else if (result.StatusCode == HttpStatusCode.BadRequest)
            {
                return this.View("Error", new ErrorViewModel { RequestId = "Tag invalid or already exists" });
            }
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
        ExceptionHelper.CheckObjectForNull(taskViewModel);

        if (this.ModelState.IsValid)
        {
            var result = await this.apiService.UpdateTaskAsync(new TaskModel(taskViewModel));
            if (result.StatusCode == HttpStatusCode.NoContent)
            {
                return this.RedirectToAction("GetTasks", "TodoLists", new { todoListId = taskViewModel.TodoListId });
            }
            else if (result.StatusCode == HttpStatusCode.NotFound)
            {
                return this.NotFound();
            }
            else if (result.StatusCode == HttpStatusCode.BadRequest)
            {
                return this.View("Error", new ErrorViewModel { RequestId = "Failed to update task: Invalid input." });
            }

            throw new InvalidOperationException("Failed to update the task.");
        }

        return this.View(taskViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> EditComment(int taskId, string oldComment, string newComment)
    {
        if (this.ModelState.IsValid && !string.IsNullOrWhiteSpace(newComment) && !string.IsNullOrEmpty(newComment))
        {
            var result = await this.apiService.UpdateCommentInTaskAsync(taskId, oldComment, newComment);
            if (result.StatusCode == HttpStatusCode.NoContent)
            {
                return this.RedirectToAction("Details", new { taskId });
            }
            else if (result.StatusCode == HttpStatusCode.NotFound)
            {
                return this.NotFound();
            }
            else if (result.StatusCode == HttpStatusCode.BadRequest)
            {
                return this.View("Error", new ErrorViewModel { RequestId = "Failed to update comment: Invalid input or such comment already exists." });
            }

            throw new InvalidOperationException("Failed to update the comment.");
        }

        return this.View("Error", new ErrorViewModel { RequestId = "Invalid Model State. New comment can't be empty or contain only whitespaces" });
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

            var result = await this.apiService.RemoveCommentFromTaskAsync(taskId, comment);
            if (result.StatusCode == HttpStatusCode.NoContent)
            {
                return this.RedirectToAction("Details", new { taskId });
            }
            else if (result.StatusCode == HttpStatusCode.NotFound)
            {
                return this.NotFound();
            }
            else if (result.StatusCode == HttpStatusCode.BadRequest)
            {
                return this.View("Error", new ErrorViewModel { RequestId = "Failed to remove comment: such comment doesn't exists." });
            }

            throw new InvalidOperationException("Failed to remove the comment from the task.");
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

            var result = await this.apiService.RemoveTagFromTaskAsync(taskId, tag);
            if (result.StatusCode == HttpStatusCode.NoContent)
            {
                return this.RedirectToAction("Details", new { taskId });
            }
            else if (result.StatusCode == HttpStatusCode.NotFound)
            {
                return this.NotFound();
            }
            else if (result.StatusCode == HttpStatusCode.BadRequest)
            {
                return this.View("Error", new ErrorViewModel { RequestId = "Failed to remove tag: such tag doesn't exists." });
            }

            throw new InvalidOperationException("Failed to remove the tag from the task.");
        }

        return this.View("Error", new ErrorViewModel { RequestId = "Invalid Model State" });
    }

    public async Task<IActionResult> AssignTask(int taskId, string assigneeId)
    {
        if (this.ModelState.IsValid)
        {
            ApplicationUser user = await this.userManager.FindByIdAsync(assigneeId);
            if (user is null)
            {
                return this.View("Error", new ErrorViewModel { RequestId = "Such user doesn't exist" });
            }

            var result = await this.apiService.UpdateAssigneeAsync(taskId, assigneeId);
            if (result.StatusCode == HttpStatusCode.NoContent)
            {
                return this.RedirectToAction("Details", new { taskId });
            }
            else if (result.StatusCode == HttpStatusCode.NotFound)
            {
                return this.NotFound();
            }

            throw new InvalidOperationException("Failed to assign task.");
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

            var result = await this.apiService.UpdateStatusOfTaskAsync(taskId, statusId);
            if (result.StatusCode == HttpStatusCode.NoContent)
            {
                return this.RedirectToAction("Details", new { taskId });
            }
            else if (result.StatusCode == HttpStatusCode.NotFound)
            {
                return this.NotFound();
            }
            else if (result.StatusCode == HttpStatusCode.BadRequest)
            {
                return this.View("Error", new ErrorViewModel { RequestId = "Failed to change status: Invalid status ID." });
            }

            throw new InvalidOperationException("Failed to change the status of the task.");
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

            var result = await this.apiService.DeleteTaskAsync(taskId);
            if (result.StatusCode == HttpStatusCode.NoContent)
            {
                return this.RedirectToAction("GetTasks", "TodoLists", new { todoListId = task.TodoListId });
            }
            else if (result.StatusCode == HttpStatusCode.NotFound)
            {
                return this.NotFound();
            }
            else if (result.StatusCode == HttpStatusCode.BadRequest)
            {
                return this.View("Error", new ErrorViewModel { RequestId = "Failed to delete task: Invalid task ID." });
            }

            return this.RedirectToAction("GetTasks", "TodoLists", new { todoListId = task.TodoListId });
        }

        return this.View("Error", new ErrorViewModel { RequestId = "Invalid Model State" });
    }
}
