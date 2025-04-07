using TodoListApp.WebApi.Entities;
using TodoListApp.WebApi.Repositories;

namespace TodoListApp.WebApi.Services;

internal class TaskService : ITaskService
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

    public async Task<Models.Task?> GetTaskByIdAsync(int id)
    {
        var task = await this.taskRepository.GetByIdAsync(id);
        return task is null ? null : new Models.Task(task);
    }

    public async Task UpdateTaskAsync(Models.Task taskToUpdate)
    {
        ArgumentNullException.ThrowIfNull(taskToUpdate);

        TaskEntity? existingTask = await this.taskRepository.GetByIdAsync(taskToUpdate.Id);
        if (existingTask is not null)
        {
            existingTask.Title = taskToUpdate.Title;
            existingTask.Description = taskToUpdate.Description;
            existingTask.CreationDate = taskToUpdate.CreationDate;
            existingTask.DueDate = taskToUpdate.DueDate;
            existingTask.Comments = taskToUpdate.Comments;
            existingTask.Tags = taskToUpdate.Tags;
            existingTask.AssigneeId = taskToUpdate.AssigneeId;
            existingTask.StatusId = taskToUpdate.Status.Id;

            _ = this.taskRepository.UpdateAsync(existingTask);
        }
    }

    public async Task DeleteTaskAsync(Models.Task taskToDelete)
    {
        TaskEntity? existingTask = await this.taskRepository.GetByIdAsync(taskToDelete.Id);

        if (existingTask is not null)
        {
            _ = this.taskRepository.DeleteByIdAsync(taskToDelete.Id);
        }
    }
}
