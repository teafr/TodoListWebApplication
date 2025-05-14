using System.Text.Json;
using Serilog;
using TodoListApp.Database.Entities;
using TodoListApp.Database.Repositories;
using TodoListApp.WebApi.Extensions;

namespace TodoListApp.WebApi.Services;

public class TaskService : ITaskService
{
    private readonly IRepository<TaskEntity> taskRepository;

    public TaskService(IRepository<TaskEntity> taskRepository)
    {
        this.taskRepository = taskRepository;
    }

    public async Task<List<Models.Task>> GetAllTasksAsync()
    {
        Log.Debug("Try to get all tasks.");

        List<TaskEntity>? tasks = await this.taskRepository.GetAsync();
        return tasks?.Select(task => new Models.Task(task))?.ToList() ?? new List<Models.Task>();
    }

    public async Task<List<Models.Task>> GetTasksByUserIdAsync(string userId)
    {
        Log.Debug("Try to get tasks by user id {0}.", userId);

        List<TaskEntity>? tasks = await this.taskRepository.GetAsync();
        return tasks?.Where(task => task.AssigneeId == userId).Select(task => new Models.Task(task))?.ToList() ?? new List<Models.Task>();
    }

    public async Task<Models.Task?> GetTaskByIdAsync(int id)
    {
        Log.Debug("Try to get task by id {0}.", id);

        var task = await this.taskRepository.GetByIdAsync(id);
        if (task is not null)
        {
            Log.Information("Task by id {0} was found.", id);
            return new Models.Task(task);
        }

        return null;
    }

    public async Task<List<string>> GetTagsByUserIdAsync(string userId)
    {
        Log.Debug("Try to get tags by user id {0}.", userId);

        List<TaskEntity>? tasks = await this.taskRepository.GetAsync();
        return tasks?.Where(task => task.AssigneeId == userId)
                     .SelectMany(task => JsonSerializer.Deserialize<List<string>>(task.Tags ?? string.Empty) ?? new List<string>())
                     .Distinct().ToList() ?? new List<string>();
    }

    public async Task<Models.Task> CreateTaskAsync(Models.Task task)
    {
        Log.Debug("Try to create new task.");

        TaskEntity existingTask = await this.taskRepository.CreateAsync(task.ToTaskEntity());
        Log.Information("Task was created.");
        return new Models.Task(existingTask);
    }

    public async Task UpdateCommentsAsync(int taskId, ICollection<string> comments)
    {
        Log.Debug("Try to update comments in task by id {0}.", taskId);

        TaskEntity? existingTask = await this.taskRepository.GetByIdAsync(taskId);
        if (existingTask is not null)
        {
            existingTask.Comments = JsonSerializer.Serialize(comments);
            this.taskRepository.Update(existingTask);
            Log.Information("Task comments were updated in task by id {0}.", taskId);
        }
        else
        {
            ThrowIfTaskNotFound(taskId);
        }
    }

    public async Task UpdateTagsAsync(int taskId, ICollection<string> tags)
    {
        Log.Debug("Try to update tags in task by id {0}.", taskId);

        TaskEntity? existingTask = await this.taskRepository.GetByIdAsync(taskId);
        if (existingTask is not null)
        {
            existingTask.Tags = JsonSerializer.Serialize(tags);
            this.taskRepository.Update(existingTask);
            Log.Information("Task tags were updated in task by id {0}.", taskId);
        }
        else
        {
            ThrowIfTaskNotFound(taskId);
        }
    }

    public async Task UpdateTaskAsync(Models.Task taskToUpdate)
    {
        ArgumentNullException.ThrowIfNull(taskToUpdate);
        Log.Debug("Try to update task by id {0}.", taskToUpdate.Id);

        TaskEntity? existingTask = await this.taskRepository.GetByIdAsync(taskToUpdate.Id);
        if (existingTask is not null)
        {
            existingTask.Title = taskToUpdate.Title;
            existingTask.Description = taskToUpdate.Description;
            existingTask.DueDate = taskToUpdate.DueDate;
            existingTask.AssigneeId = taskToUpdate.AssigneeId;
            existingTask.StatusId = taskToUpdate.Status.Id;

            this.taskRepository.Update(existingTask);
            Log.Information("Task by id {0} was updated.", taskToUpdate.Id);
        }
        else
        {
            ThrowIfTaskNotFound(taskToUpdate.Id);
        }
    }

    public async Task UpdateStatusOfTaskAsync(int taskId, int statusId)
    {
        Log.Debug("Try to update status of task by id {0}.", taskId);

        TaskEntity? existingTask = await this.taskRepository.GetByIdAsync(taskId);
        if (existingTask is not null)
        {
            existingTask.StatusId = statusId;
            this.taskRepository.Update(existingTask);
            Log.Information("Status of task by id {0} was updated.", taskId);
        }
        else
        {
            ThrowIfTaskNotFound(taskId);
        }
    }

    public async Task UpdateAssigneeAsync(int taskId, string assigneeId)
    {
        Log.Debug("Try to update assignee of task by id {0}.", taskId);

        TaskEntity? existingTask = await this.taskRepository.GetByIdAsync(taskId);
        if (existingTask is not null)
        {
            existingTask.AssigneeId = assigneeId;
            this.taskRepository.Update(existingTask);
            Log.Information("Assignee of task by id {0} was updated.", taskId);
        }
        else
        {
            ThrowIfTaskNotFound(taskId);
        }
    }

    public async Task DeleteTaskByIdAsync(int id)
    {
        Log.Debug("Try to delete task by id {0}.", id);

        TaskEntity? existingTask = await this.taskRepository.GetByIdAsync(id);

        if (existingTask is not null)
        {
            this.taskRepository.Delete(existingTask);
            Log.Information("Task by id {0} was deleted.", id);
        }
        else
        {
            ThrowIfTaskNotFound(id);
        }
    }

    private static void ThrowIfTaskNotFound(int taskId)
    {
        throw new InvalidDataException($"Task by id {taskId} was not found.");
    }
}
