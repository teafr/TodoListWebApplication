namespace TodoListApp.WebApi.Services;

public interface ITaskService
{
    Task DeleteTaskAsync(Models.Task taskToDelete);

    Task<List<Models.Task>> GetAllTasksAsync();

    Task<Models.Task?> GetTaskByIdAsync(int id);

    Task UpdateTaskAsync(Models.Task taskToUpdate);
}
