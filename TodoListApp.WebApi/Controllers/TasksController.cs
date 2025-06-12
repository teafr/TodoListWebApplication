using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using TodoListApp.WebApi.Extensions;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApi.Services;

namespace TodoListApp.WebApi.Controllers;

[Authorize]
public class TasksController : BaseController
{
    private readonly ITaskService taskService;

    public TasksController(ITaskService taskService)
    {
        this.taskService = taskService;
    }

    /// <summary>
    /// Get all tasks.
    /// </summary>
    /// <returns>All tasks from DB.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> Get()
    {
        List<Models.Task> tasks = await this.taskService.GetAllTasksAsync();
        Log.Debug("Successfully have got all tasks.");
        return this.Ok(tasks.Select(task => task.ToTaskApiModel()));
    }

    /// <summary>
    /// Get all tasks by user id.
    /// </summary>
    /// <param name="userId">Id of the user.</param>
    /// <returns>All tasks of user.</returns>
    [HttpGet("user/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> GetByUserId(string userId)
    {
        List<Models.Task> tasks = await this.taskService.GetTasksByUserIdAsync(userId);
        Log.Debug("Successfully have got tasks by user id {0}.", userId);
        return this.Ok(tasks.Select(task => task.ToTaskApiModel()));
    }

    /// <summary>
    /// Get task by id.
    /// </summary>
    /// <param name="taskId">Id of the task.</param>
    /// <returns>Task.</returns>
    [HttpGet("{taskId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> GetById(int taskId)
    {
        Models.Task? task = await this.taskService.GetTaskByIdAsync(taskId);
        if (task is null)
        {
            return this.NotFound();
        }

        return this.Ok(task.ToTaskApiModel());
    }

    /// <summary>
    /// Get all tags by user id.
    /// </summary>
    /// <param name="userId">Id of the user.</param>
    /// <returns>Tags which were created by user.</returns>
    [HttpGet("{userId}/tags")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> GetTags(string userId)
    {
        return this.Ok(await this.taskService.GetTagsByUserIdAsync(userId));
    }

    /// <summary>
    /// Create new task.
    /// </summary>
    /// <param name="apiTask">Task model.</param>
    /// <returns>Created task.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> Create(TaskApiModel apiTask)
    {
        try
        {
            Models.Task newTask = await this.taskService.CreateTaskAsync(new Models.Task(apiTask));
            return this.CreatedAtAction(nameof(this.GetById), new { taskId = newTask.Id }, newTask.ToTaskApiModel());
        }
        catch (ArgumentNullException ex)
        {
            Log.Error(ex, "Task wasn't created. {Message}", new { ex.Message });
            return this.BadRequest();
        }
    }

    /// <summary>
    /// Add tag to task.
    /// </summary>
    /// <param name="taskId">Id of the task.</param>
    /// <param name="tag">Tag to add.</param>
    [HttpPost("{taskId:int}/tag/{tag}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> AddTag(int taskId, string tag)
    {
        Models.Task? task = await this.taskService.GetTaskByIdAsync(taskId);
        if (task is null)
        {
            return this.NotFound();
        }

        task.Tags!.Add(tag);
        return await this.ExecuteWithValidation(() => this.taskService.UpdateTagsAsync(taskId, task.Tags!));
    }

    /// <summary>
    /// Add comment to task.
    /// </summary>
    /// <param name="taskId">Id of the task.</param>
    /// <param name="comment">Comment to add.</param>
    [HttpPost("{taskId:int}/comment/{comment}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> AddComment(int taskId, string comment)
    {
        Models.Task? task = await this.taskService.GetTaskByIdAsync(taskId);
        if (task is null)
        {
            return this.NotFound();
        }

        task.Comments!.Add(comment);
        return await this.ExecuteWithValidation(() => this.taskService.UpdateCommentsAsync(taskId, task.Comments!));
    }

    /// <summary>
    /// Update task.
    /// </summary>
    /// <param name="apiTask">Task model.</param>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> Update(TaskApiModel apiTask)
    {
        if (apiTask is null)
        {
            return this.BadRequest();
        }

        Models.Task? existingTask = await this.taskService.GetTaskByIdAsync(apiTask.Id);
        if (existingTask is null)
        {
            return this.NotFound();
        }

        return await this.ExecuteWithValidation(() => this.taskService.UpdateTaskAsync(new Models.Task(apiTask)));
    }

    /// <summary>
    /// Update status of task.
    /// </summary>
    /// <param name="taskId">Id of the task.</param>
    /// <param name="statusId">Id of the status.</param>
    [HttpPut("{taskId:int}/status/{statusId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> UpdateStatus(int taskId, int statusId)
    {
        Models.Task? task = await this.taskService.GetTaskByIdAsync(taskId);
        if (task is null)
        {
            return this.NotFound();
        }

        return await this.ExecuteWithValidation(() => this.taskService.UpdateStatusOfTaskAsync(taskId, statusId));
    }

    /// <summary>
    /// Update assignee of task.
    /// </summary>
    /// <param name="taskId">Id of the task.</param>
    /// <param name="userId">Id of the user.</param>
    [HttpPut("{taskId:int}/user/{userId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> UpdateAssignee(int taskId, string userId)
    {
        Models.Task? task = await this.taskService.GetTaskByIdAsync(taskId);
        if (task is null)
        {
            return this.NotFound();
        }

        return await this.ExecuteWithValidation(() => this.taskService.UpdateAssigneeAsync(taskId, userId));
    }

    /// <summary>
    /// Update comment of task.
    /// </summary>
    /// <param name="taskId">Id of the task.</param>
    /// <param name="oldComment">Comment to edit.</param>
    /// <param name="newComment">New comment.</param>
    [HttpPut("{taskId:int}/comment/{oldComment}/{newComment}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> UpdateComment(int taskId, string oldComment, string newComment)
    {
        Models.Task? task = await this.taskService.GetTaskByIdAsync(taskId);
        if (task is null)
        {
            return this.NotFound();
        }

        if (task.Comments?.Remove(oldComment) ?? false)
        {
            task.Comments.Add(newComment);
            return await this.ExecuteWithValidation(() => this.taskService.UpdateCommentsAsync(taskId, task.Comments ?? new List<string>()));
        }
        else
        {
            Log.Warning("Comment wasn't updated.");
            return this.BadRequest();
        }
    }

    [HttpDelete("{taskId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> Delete(int taskId)
    {
        Models.Task? task = await this.taskService.GetTaskByIdAsync(taskId);
        if (task is null)
        {
            return this.NotFound();
        }

        return await this.ExecuteWithValidation(() => this.taskService.DeleteTaskByIdAsync(taskId));
    }

    /// <summary>
    /// Delete tag from task.
    /// </summary>
    /// <param name="taskId">Id of the task.</param>
    /// <param name="tag">Tag to delete</param>
    [HttpDelete("{taskId:int}/tag/{tag}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> DeleteTag(int taskId, string tag)
    {
        Models.Task? task = await this.taskService.GetTaskByIdAsync(taskId);
        if (task is null)
        {
            return this.NotFound();
        }

        if (task.Tags?.Remove(tag) ?? false)
        {
            return await this.ExecuteWithValidation(() => this.taskService.UpdateTagsAsync(taskId, task.Tags));
        }

        Log.Warning("Tag wasn't deleted.");
        return this.BadRequest();
    }

    /// <summary>
    /// Delete comment from task.
    /// </summary>
    /// <param name="taskId">Id of the task.</param>
    /// <param name="comment">Comment to delete.</param>
    [HttpDelete("{taskId:int}/comment/{comment}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> DeleteComment(int taskId, string comment)
    {
        Models.Task? task = await this.taskService.GetTaskByIdAsync(taskId);
        if (task is null)
        {
            return this.NotFound();
        }

        if (task.Comments?.Remove(comment) ?? false)
        {
            return await this.ExecuteWithValidation(() => this.taskService.UpdateCommentsAsync(taskId, task.Comments));
        }

        Log.Warning("Comment wasn't deleted.");
        return this.BadRequest();
    }

    public override NotFoundResult NotFound()
    {
        Log.Warning("Task was not found.");
        return base.NotFound();
    }
}
