using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApi.Extensions;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApi.Services;

namespace TodoListApp.WebApi.Controllers
{
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
        public async Task<IActionResult> GetByUserId(string userId)
        {
            List<Models.Task> tasks = await this.taskService.GetAllTasksAsync();
            return this.Ok(tasks.Where(task => task.AssigneeId == userId).Select(task => task.ToTaskApiModel()));
        }

        [HttpGet("{taskId}/comments")]
        public async Task<IActionResult> GetComments(int taskId)
        {
            Models.Task? task = await this.taskService.GetTaskByIdAsync(taskId);
            if (task is null)
            {
                return this.NotFound();
            }

            return this.Ok(task.Comments);
        }

        [HttpGet("{taskId}/tags")]
        public async Task<IActionResult> GetTags(int taskId)
        {
            Models.Task? task = await this.taskService.GetTaskByIdAsync(taskId);
            if (task is null)
            {
                return this.NotFound();
            }

            return this.Ok(task.Tags);
        }

        [HttpPost("{taskId:int}/tag/{tag}")]
        public async Task<IActionResult> AddTag(int taskId, string tag)
        {
            Models.Task? task = await this.taskService.GetTaskByIdAsync(taskId);
            if (task is null)
            {
                return this.NotFound();
            }

            if (task.Tags is null)
            {
                task.Tags = new Collection<string>();
            }

            task.Tags.Add(tag);
            await this.taskService.UpdateTaskAsync(task);

            return this.NoContent();
        }

        [HttpPost("{taskId:int}/comment/{comment}")]
        public async Task<IActionResult> AddComment(int taskId, string comment)
        {
            Models.Task? task = await this.taskService.GetTaskByIdAsync(taskId);
            if (task is null)
            {
                return this.NotFound();
            }

            if (task.Comments is null)
            {
                task.Comments = new Collection<string>();
            }

            if (task.Comments.FirstOrDefault(x => x == comment) is null)
            {
                task.Comments.Add(comment);
            }

            await this.taskService.UpdateTaskAsync(task);
            return this.NoContent();
        }

        [HttpPut]
        public async Task<IActionResult> Update(TaskApiModel apiTask)
        {
            ArgumentNullException.ThrowIfNull(apiTask);

            Models.Task? existingTask = await this.taskService.GetTaskByIdAsync(apiTask.Id);
            if (existingTask is null)
            {
                return this.NotFound();
            }

            await this.taskService.UpdateTaskAsync(new Models.Task(apiTask));
            return this.NoContent();
        }

        [HttpPut("{taskId:int}/status/{statusId:int}")]
        public async Task<IActionResult> UpdateStatus(int taskId, int statusId)
        {
            Models.Task? task = await this.taskService.GetTaskByIdAsync(taskId);
            if (task is null)
            {
                return this.NotFound();
            }

            task.Status.Id = statusId;
            await this.taskService.UpdateTaskAsync(task);

            return this.NoContent();
        }

        [HttpPut("{taskId:int}/comment/{oldComment}/{newComment}")]
        public async Task<IActionResult> UpdateComment(int taskId, string oldComment, string newComment)
        {
            Models.Task? task = await this.taskService.GetTaskByIdAsync(taskId);
            if (task is null)
            {
                return this.NotFound();
            }

            if (task.Comments?.Remove(task.Comments.FirstOrDefault(oldComment)) ?? false)
            {
                task.Comments.Add(newComment);
            }

            await this.taskService.UpdateTaskAsync(task);
            return this.NoContent();
        }

        [HttpDelete("{taskId:int}")]
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
        public async Task<IActionResult> DeleteTag(int taskId, string tag)
        {
            Models.Task? task = await this.taskService.GetTaskByIdAsync(taskId);
            if (task is null || tag is null)
            {
                return this.NotFound();
            }

            if (task.Tags is not null)
            {
                _ = task.Tags.Remove(tag);
                await this.taskService.UpdateTaskAsync(task);
            }

            return this.NoContent();
        }

        [HttpDelete("{taskId:int}/comment/{comment}")]
        public async Task<IActionResult> DeleteComment(int taskId, string comment)
        {
            Models.Task? task = await this.taskService.GetTaskByIdAsync(taskId);
            if (task is null || comment is null)
            {
                return this.NotFound();
            }

            if (task.Comments is not null)
            {
                _ = task.Comments.Remove(comment);
                await this.taskService.UpdateTaskAsync(task);
            }

            return this.NoContent();
        }
    }
}
