using System.Globalization;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Serilog;
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
                Log.Warning("User isn't authorized.");
                return this.View(new ListOfTasks(new List<TaskModel>(), new TaskFilterModel()));
            }

            List<TaskModel>? tasks;

            try
            {
                tasks = await this.apiService.GetTasksByUserIdAsync(userId);
                Log.Information("User {UserId} retrieved tasks", userId);
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

            Log.Information("User {UserId} retrieved task details for task ID {TaskId}", this.userManager.GetUserId(this.User), taskId);
            return this.View(task.ToTaskViewModel(await this.userManager.FindByIdAsync(task.AssigneeId)));
        }

        return this.View("Error", new ErrorViewModel { RequestId = "Invalid Model State" });
    }

    public async Task<IActionResult> GetTags()
    {
        string userId = this.userManager.GetUserId(this.User) ?? throw new InvalidOperationException("User is not authenticated.");

        List<string>? tags = await this.apiService.GetTagsByUserIdAsync(userId);
        Log.Information("User {UserId} retrieved tags", userId);

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
            var currentUser = await this.userManager.GetUserAsync(this.User);
            taskViewModel.Assignee = currentUser;
            taskViewModel.Status = new StatusViewModel() { Id = (int)StatusOfTask.NotStarted, Name = string.Empty };
            taskViewModel.CreationDate = DateTime.Now;

            var result = await this.apiService.CreateTaskAsync(new TaskModel(taskViewModel));
            if (result.StatusCode == HttpStatusCode.Created)
            {
                Log.Information("User {UserId} created a new task with title '{TaskTitle}'", currentUser.Id, taskViewModel.Title);
                return this.RedirectToAction("GetTasks", "TodoLists", new { todoListId = taskViewModel.TodoListId });
            }
            else if (result.StatusCode == HttpStatusCode.BadRequest)
            {
                Log.Warning("User {UserId} attempted to create a task with invalid data", currentUser.Id);
                this.ModelState.AddModelError(string.Empty, "Invalid task");
            }

            Log.Error("User by id {UserId} failed to create task. Status code: ", currentUser.Id, result.StatusCode);
            return this.View("Error", new ErrorViewModel { RequestId = "We couldn't create a new task. Please try again later." });
        }

        return this.View(new TaskViewModel() { TodoListId = taskViewModel.TodoListId });
    }

    [HttpPost]
    public async Task<IActionResult> AddComment(int taskId, string comment)
    {
        if (this.ModelState.IsValid)
        {
            var currentUserId = this.userManager.GetUserId(this.User);
            var result = await this.apiService.AddCommentAsync(taskId, comment);
            if (result.StatusCode == HttpStatusCode.NoContent)
            {
                Log.Information("User {UserId} added a comment to task ID {TaskId}", currentUserId, taskId);
                return this.RedirectToAction("Details", new { taskId });
            }
            else if (result.StatusCode == HttpStatusCode.NotFound)
            {
                return this.NotFound();
            }
            else if (result.StatusCode == HttpStatusCode.BadRequest)
            {
                Log.Warning("User {UserId} attempted to add an invalid comment to task ID {TaskId}", currentUserId, taskId);
                return this.View("Error", new ErrorViewModel { RequestId = "Comment invalid or already exists" });
            }

            Log.Error("User {UserId} failed to add a comment to task ID {TaskId}. Status code: ", currentUserId, taskId, result.StatusCode);
            return this.View("Error", new ErrorViewModel { RequestId = "We couldn't add a comment to the task. Please try again later." });
        }

        return this.View("Error", new ErrorViewModel { RequestId = "Invalid Model State" });
    }

    [HttpPost]
    public async Task<IActionResult> AddTag(int taskId, string tag)
    {
        if (this.ModelState.IsValid)
        {
            var currentUserId = this.userManager.GetUserId(this.User);
            var result = await this.apiService.AddTagAsync(taskId, tag);
            if (result.StatusCode == HttpStatusCode.NoContent)
            {
                Log.Information("User {UserId} added tag '{Tag}' to task ID {TaskId}", currentUserId, tag, taskId);
                return this.RedirectToAction("Details", new { taskId });
            }
            else if (result.StatusCode == HttpStatusCode.NotFound)
            {
                return this.NotFound();
            }
            else if (result.StatusCode == HttpStatusCode.BadRequest)
            {
                Log.Warning("User {UserId} attempted to add an invalid or duplicate tag '{Tag}' to task ID {TaskId}", currentUserId, tag, taskId);
                return this.View("Error", new ErrorViewModel { RequestId = "Tag invalid or already exists" });
            }

            Log.Error("User {UserId} failed to add tag '{Tag}' to task ID {TaskId}. Status code: ", currentUserId, tag, taskId, result.StatusCode);
            return this.View("Error", new ErrorViewModel { RequestId = "We couldn't add a tag to the task. Please try again later." });
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
            var currentUser = await this.userManager.GetUserAsync(this.User);
            var result = await this.apiService.UpdateTaskAsync(new TaskModel(taskViewModel));
            if (result.StatusCode == HttpStatusCode.NoContent)
            {
                Log.Information("User {UserId} updated task with ID {TaskId} and title '{TaskTitle}'", currentUser.Id, taskViewModel.Id, taskViewModel.Title);
                return this.RedirectToAction("GetTasks", "TodoLists", new { todoListId = taskViewModel.TodoListId });
            }
            else if (result.StatusCode == HttpStatusCode.NotFound)
            {
                return this.NotFound();
            }
            else if (result.StatusCode == HttpStatusCode.BadRequest)
            {
                Log.Warning("User {UserId} attempted to update task with ID {TaskId} with invalid data", currentUser.Id, taskViewModel.Id);
                return this.View("Error", new ErrorViewModel { RequestId = "Failed to update task: Invalid input." });
            }

            Log.Error("User {UserId} failed to update task with ID {TaskId}. Status code: ", currentUser.Id, taskViewModel.Id, result.StatusCode);
            return this.View("Error", new ErrorViewModel { RequestId = "We couldn't update the task. Please try again later." });
        }

        return this.View(taskViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> EditComment(int taskId, string oldComment, string newComment)
    {
        if (this.ModelState.IsValid && !string.IsNullOrWhiteSpace(newComment) && !string.IsNullOrEmpty(newComment))
        {
            var currentUserId = this.userManager.GetUserId(this.User);
            var result = await this.apiService.UpdateCommentInTaskAsync(taskId, oldComment, newComment);
            if (result.StatusCode == HttpStatusCode.NoContent)
            {
                Log.Information("User {UserId} updated comment in task ID {TaskId} from '{OldComment}' to '{NewComment}'", currentUserId, taskId, oldComment, newComment);
                return this.RedirectToAction("Details", new { taskId });
            }
            else if (result.StatusCode == HttpStatusCode.NotFound)
            {
                return this.NotFound();
            }
            else if (result.StatusCode == HttpStatusCode.BadRequest)
            {
                Log.Warning("User {UserId} attempted to update comment in task ID {TaskId} with invalid data", currentUserId, taskId);
                return this.View("Error", new ErrorViewModel { RequestId = "Failed to update comment: Invalid input or such comment already exists." });
            }

            Log.Error("User {UserId} failed to update comment in task ID {TaskId}. Status code: ", currentUserId, taskId, result.StatusCode);
            return this.View("Error", new ErrorViewModel { RequestId = "We couldn't update the comment. Please try again later." });
        }

        return this.View("Error", new ErrorViewModel { RequestId = "Invalid Model State. New comment can't be empty or contain only whitespaces" });
    }

    public async Task<IActionResult> RemoveComment(int taskId, string comment)
    {
        if (this.ModelState.IsValid)
        {
            var currentUserId = this.userManager.GetUserId(this.User);
            var result = await this.apiService.RemoveCommentFromTaskAsync(taskId, comment);
            if (result.StatusCode == HttpStatusCode.NoContent)
            {
                Log.Information("User {UserId} removed comment '{Comment}' from task ID {TaskId}", currentUserId, comment, taskId);
                return this.RedirectToAction("Details", new { taskId });
            }
            else if (result.StatusCode == HttpStatusCode.NotFound)
            {
                return this.NotFound();
            }
            else if (result.StatusCode == HttpStatusCode.BadRequest)
            {
                Log.Warning("User {UserId} attempted to remove a comment '{Comment}' from task ID {TaskId} that doesn't exist", currentUserId, comment, taskId);
                return this.View("Error", new ErrorViewModel { RequestId = "Failed to remove comment: such comment doesn't exists." });
            }

            Log.Error("User {UserId} failed to remove comment '{Comment}' from task ID {TaskId}. Status code: ", currentUserId, comment, taskId, result.StatusCode);
            return this.View("Error", new ErrorViewModel { RequestId = "We couldn't remove the comment. Please try again later." });
        }

        return this.View("Error", new ErrorViewModel { RequestId = "Invalid Model State" });
    }

    public async Task<IActionResult> RemoveTag(int taskId, string tag)
    {
        if (this.ModelState.IsValid)
        {
            var currentUserId = this.userManager.GetUserId(this.User);
            var result = await this.apiService.RemoveTagFromTaskAsync(taskId, tag);
            if (result.StatusCode == HttpStatusCode.NoContent)
            {
                Log.Information("User {UserId} removed tag '{Tag}' from task ID {TaskId}", currentUserId, tag, taskId);
                return this.RedirectToAction("Details", new { taskId });
            }
            else if (result.StatusCode == HttpStatusCode.NotFound)
            {
                return this.NotFound();
            }
            else if (result.StatusCode == HttpStatusCode.BadRequest)
            {
                Log.Warning("User {UserId} attempted to remove a tag '{Tag}' from task ID {TaskId} that doesn't exist", currentUserId, tag, taskId);
                return this.View("Error", new ErrorViewModel { RequestId = "Failed to remove tag: such tag doesn't exists." });
            }

            Log.Error("User {UserId} failed to remove tag '{Tag}' from task ID {TaskId}. Status code: ", currentUserId, tag, taskId, result.StatusCode);
            return this.View("Error", new ErrorViewModel { RequestId = "We couldn't remove the tag. Please try again later." });
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
                Log.Warning("User with ID {AssigneeId} does not exist", assigneeId);
                return this.View("Error", new ErrorViewModel { RequestId = "Such user doesn't exist" });
            }

            var currentUserId = this.userManager.GetUserId(this.User);
            var result = await this.apiService.UpdateAssigneeAsync(taskId, assigneeId);
            if (result.StatusCode == HttpStatusCode.NoContent)
            {
                Log.Information("User {UserId} assigned task ID {TaskId} to user {AssigneeId}", currentUserId, taskId, assigneeId);
                return this.RedirectToAction("Details", new { taskId });
            }
            else if (result.StatusCode == HttpStatusCode.NotFound)
            {
                return this.NotFound();
            }

            Log.Error("User {UserId} failed to assign task ID {TaskId} to user {AssigneeId}. Status code: ", currentUserId, taskId, assigneeId, result.StatusCode);
            return this.View("Error", new ErrorViewModel { RequestId = "We couldn't assign the task. Please try again later." });
        }

        return this.View("Error", new ErrorViewModel { RequestId = "Invalid Model State" });
    }

    public async Task<IActionResult> ChangeStatus(int taskId, int statusId)
    {
        if (this.ModelState.IsValid)
        {
            var currentUserId = this.userManager.GetUserId(this.User);
            var result = await this.apiService.UpdateStatusOfTaskAsync(taskId, statusId);
            if (result.StatusCode == HttpStatusCode.NoContent)
            {
                Log.Information("User {UserId} changed status of task ID {TaskId} to status ID {StatusId}", currentUserId, taskId, statusId);
                return this.RedirectToAction("Details", new { taskId });
            }
            else if (result.StatusCode == HttpStatusCode.NotFound)
            {
                return this.NotFound();
            }
            else if (result.StatusCode == HttpStatusCode.BadRequest)
            {
                Log.Warning("User {UserId} attempted to change status of task ID {TaskId} to invalid status ID {StatusId}", currentUserId, taskId, statusId);
                return this.View("Error", new ErrorViewModel { RequestId = "Failed to change status: Invalid status ID." });
            }

            Log.Error("User {UserId} failed to change status of task ID {TaskId} to status ID {StatusId}. Status code: ", currentUserId, taskId, statusId, result.StatusCode);
            return this.View("Error", new ErrorViewModel { RequestId = "We couldn't change the status of the task. Please try again later." });
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

            var currentUserId = this.userManager.GetUserId(this.User);
            var result = await this.apiService.DeleteTaskAsync(taskId);
            if (result.StatusCode == HttpStatusCode.NoContent)
            {
                Log.Information("User {UserId} deleted task with ID {TaskId}", currentUserId, taskId);
                return this.RedirectToAction("GetTasks", "TodoLists", new { todoListId = task.TodoListId });
            }
            else if (result.StatusCode == HttpStatusCode.NotFound)
            {
                return this.NotFound();
            }

            Log.Error("User {UserId} failed to delete task with ID {TaskId}. Status code: ", currentUserId, taskId, result.StatusCode);
            return this.View("Error", new ErrorViewModel { RequestId = "We couldn't delete the task. Please try again later." });
        }

        return this.View("Error", new ErrorViewModel { RequestId = "Invalid Model State" });
    }

    public override NotFoundResult NotFound()
    {
        Log.Warning("Resource not found. User: {UserId}, Path: {Path}", this.userManager.GetUserId(this.User), this.HttpContext.Request.Path);
        return base.NotFound();
    }
}
