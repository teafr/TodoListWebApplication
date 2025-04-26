using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApi.Extensions;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApi.Services;

namespace TodoListApp.WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService taskService;

        public TasksController(ITaskService taskService)
        {
            this.taskService = taskService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> Get()
        {
            List<Models.Task> tasks = await this.taskService.GetAllTasksAsync();
            if (tasks is null)
            {
                return this.NotFound();
            }

            return this.Ok(tasks.Select(task => task.ToTaskApiModel()));
        }

        [HttpGet("{taskId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        [HttpGet("user/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> GetByUserId(string userId)
        {
            return this.Ok(await this.taskService.GetTasksByUserIdAsync(userId));
        }

        [HttpGet("{userId}/tags")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> GetTags(string userId)
        {
            return this.Ok(await this.taskService.GetTagsByUserIdAsync(userId));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> Create(TaskApiModel apiTask)
        {
            Models.Task newTask = await this.taskService.CreateTaskAsync(new Models.Task(apiTask));
            return this.CreatedAtAction(nameof(this.GetById), new { taskId = newTask.Id }, newTask.ToTaskApiModel());
        }

        [HttpPost("{taskId:int}/tag/{tag}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> AddTag(int taskId, string tag)
        {
            Models.Task? task = await this.taskService.GetTaskByIdAsync(taskId);
            if (task is null)
            {
                return this.NotFound();
            }

            task.Tags!.Add(tag);
            await this.taskService.UpdateTagsAsync(taskId, task.Tags);

            return this.NoContent();
        }

        [HttpPost("{taskId:int}/comment/{comment}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> AddComment(int taskId, string comment)
        {
            Models.Task? task = await this.taskService.GetTaskByIdAsync(taskId);
            if (task is null)
            {
                return this.NotFound();
            }

            task.Comments!.Add(comment);
            await this.taskService.UpdateCommentsAsync(taskId, task.Comments);
            return this.NoContent();
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
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

            await this.taskService.UpdateTaskAsync(new Models.Task(apiTask));
            return this.NoContent();
        }

        [HttpPut("{taskId:int}/status/{statusId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> UpdateStatus(int taskId, int statusId)
        {
            Models.Task? task = await this.taskService.GetTaskByIdAsync(taskId);
            if (task is null)
            {
                return this.NotFound();
            }

            await this.taskService.UpdateStatusOfTaskAsync(taskId, statusId);

            return this.NoContent();
        }

        [HttpPut("{taskId:int}/user/{userId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> UpdateUser(int taskId, string userId)
        {
            Models.Task? task = await this.taskService.GetTaskByIdAsync(taskId);
            if (task is null)
            {
                return this.NotFound();
            }

            await this.taskService.UpdateAssigneeAsync(taskId, userId);
            return this.NoContent();
        }

        [HttpPut("{taskId:int}/comment/{oldComment}/{newComment}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
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
                await this.taskService.UpdateCommentsAsync(taskId, task.Comments);
            }

            return this.NoContent();
        }

        [HttpDelete("{taskId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> Delete(int taskId)
        {
            Models.Task? task = await this.taskService.GetTaskByIdAsync(taskId);
            if (task is null)
            {
                return this.NotFound();
            }

            await this.taskService.DeleteTaskAsync(task);
            return this.NoContent();
        }

        [HttpDelete("{taskId:int}/tag/{tag}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> DeleteTag(int taskId, string tag)
        {
            Models.Task? task = await this.taskService.GetTaskByIdAsync(taskId);
            if (task is null)
            {
                return this.NotFound();
            }

            if (task.Tags is not null)
            {
                _ = task.Tags.Remove(tag);
                await this.taskService.UpdateTagsAsync(taskId, task.Tags);
            }

            return this.NoContent();
        }

        [HttpDelete("{taskId:int}/comment/{comment}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> DeleteComment(int taskId, string comment)
        {
            Models.Task? task = await this.taskService.GetTaskByIdAsync(taskId);
            if (task is null)
            {
                return this.NotFound();
            }

            if (task.Comments is not null)
            {
                _ = task.Comments.Remove(comment);
                await this.taskService.UpdateCommentsAsync(taskId, task.Comments);
            }

            return this.NoContent();
        }
    }
}
