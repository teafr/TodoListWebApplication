using TodoListApp.WebApp.Models;

namespace TodoListApp.WebApp.Services;
public interface ITaskWebApiService
{
    Task<TaskModel?> GetTaskByIdAsync(int taskId);

    Task<List<TaskModel>> GetTasksByUserIdAsync(string userId);

    Task<List<string>> GetTagsByUserIdAsync(string userId);

    Task AddCommentAsync(int taskId, string comment);

    Task AddTagAsync(int taskId, string tag);

    Task CreateTaskAsync(TaskModel task);

    Task UpdateCommentInTaskAsync(int taskId, string oldComment, string newComment);

    Task UpdateStatusOfTaskAsync(int taskId, int statusId);

    Task UpdateAssigneeAsync(int taskId, string assigneeId);

    Task UpdateTaskAsync(TaskModel task);

    Task DeleteTaskAsync(int taskId);

    Task RemoveCommentFromTaskAsync(int taskId, string comment);

    Task RemoveTagFromTaskAsync(int taskId, string tag);
}
