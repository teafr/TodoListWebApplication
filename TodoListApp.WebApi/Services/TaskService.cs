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
        List<TaskEntity>? tasks = await this.taskRepository.GetAsync();
        return tasks?.Select(task => new Models.Task(task))?.ToList() ?? new List<Models.Task>();
    }

    public async Task<List<Models.Task>> GetTasksByUserIdAsync(string userId)
    {
        List<TaskEntity>? tasks = await this.taskRepository.GetAsync();
        return tasks?.Where(task => task.AssigneeId == userId).Select(task => new Models.Task(task))?.ToList() ?? new List<Models.Task>();
    }

    public async Task<Models.Task?> GetTaskByIdAsync(int id)
    {
        var task = await this.taskRepository.GetByIdAsync(id);
        return task is null ? null : new Models.Task(task);
    }

    public async Task<List<string>> GetTagsByUserIdAsync(string userId)
    {
        List<TaskEntity>? tasks = await this.taskRepository.GetAsync();
        return tasks?.Where(task => task.AssigneeId == userId).SelectMany(task => task.Tags ?? new List<string>()).Distinct().ToList() ?? new List<string>();
    }

    public async Task<Models.Task> CreateTaskAsync(Models.Task task)
    {
        TaskEntity existingTask = await this.taskRepository.CreateAsync(task.ToTaskEntity());
        return new Models.Task(existingTask);
    }

    public async Task UpdateCommentsAsync(int taskId, ICollection<string> comments)
    {
        TaskEntity? existingTask = await this.taskRepository.GetByIdAsync(taskId);
        if (existingTask is not null && comments is not null)
        {
            existingTask.Comments = comments;
            this.taskRepository.Update(existingTask);
        }
    }

    public async Task UpdateTagsAsync(int taskId, ICollection<string> tags)
    {
        TaskEntity? existingTask = await this.taskRepository.GetByIdAsync(taskId);
        if (existingTask is not null && tags is not null)
        {
            existingTask.Tags = tags;
            this.taskRepository.Update(existingTask);
        }
    }

    public async Task UpdateTaskAsync(Models.Task taskToUpdate)
    {
        if (taskToUpdate is not null)
        {
            TaskEntity? existingTask = await this.taskRepository.GetByIdAsync(taskToUpdate.Id);
            if (existingTask is not null)
            {
                existingTask.Title = taskToUpdate.Title;
                existingTask.Description = taskToUpdate.Description;
                existingTask.DueDate = taskToUpdate.DueDate;
                existingTask.AssigneeId = taskToUpdate.AssigneeId;
                existingTask.StatusId = taskToUpdate.Status.Id;

                this.taskRepository.Update(existingTask);
            }
        }
    }

    public async Task UpdateStatusOfTaskAsync(int taskId, int statusId)
    {
        TaskEntity? existingTask = await this.taskRepository.GetByIdAsync(taskId);
        if (existingTask is not null)
        {
            existingTask.StatusId = statusId;
            this.taskRepository.Update(existingTask);
        }
    }

    public async Task UpdateAssigneeAsync(int taskId, string assigneeId)
    {
        TaskEntity? existingTask = await this.taskRepository.GetByIdAsync(taskId);
        if (existingTask is not null)
        {
            existingTask.AssigneeId = assigneeId;
            this.taskRepository.Update(existingTask);
        }
    }

    public async Task DeleteTaskAsync(Models.Task taskToDelete)
    {
        if (taskToDelete is not null)
        {
            TaskEntity? existingTask = await this.taskRepository.GetByIdAsync(taskToDelete.Id);

            if (existingTask is not null)
            {
                _ = this.taskRepository.DeleteByIdAsync(taskToDelete.Id);
            }
        }
    }
}
