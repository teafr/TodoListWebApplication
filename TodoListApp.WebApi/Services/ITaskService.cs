namespace TodoListApp.WebApi.Services;

public interface ITaskService
{
    Task<List<Models.Task>> GetAllTasksAsync();

    Task<Models.Task?> GetTaskByIdAsync(int id);

    Task<List<Models.Task>> GetTasksByUserIdAsync(string userId);

    Task<List<string>> GetTagsByUserIdAsync(string userId);

    Task<Models.Task> CreateTaskAsync(Models.Task task);

    Task UpdateTaskAsync(Models.Task taskToUpdate);

    Task UpdateStatusOfTaskAsync(int taskId, int statusId);

    Task UpdateAssigneeAsync(int taskId, string assigneeId);

    Task UpdateCommentsAsync(int taskId, ICollection<string> comments);

    Task UpdateTagsAsync(int taskId, ICollection<string> tags);

    Task DeleteTaskAsync(Models.Task taskToDelete);
}
