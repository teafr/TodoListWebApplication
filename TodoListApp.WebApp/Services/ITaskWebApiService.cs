using TodoListApp.WebApp.Models;

namespace TodoListApp.WebApp.Services;
public interface ITaskWebApiService
{
    Task<bool> AddCommentAsync(int taskId, string comment);

    Task<bool> AddTagAsync(int taskId, string tag);

    Task<bool> CreateTaskAsync(TaskModel task);

    Task<bool> DeleteTaskAsync(int taskId);

    Task<List<string>> GetTagsByUserIdAsync(string userId);

    Task<TaskModel?> GetTaskByIdAsync(int taskId);

    Task<List<TaskModel>> GetTasksByUserIdAsync(string userId);

    Task<bool> RemoveCommentFromTaskAsync(int taskId, string comment);

    Task<bool> RemoveTagFromTaskAsync(int taskId, string tag);

    Task<bool> UpdateAssigneeAsync(int taskId, string assigneeId);

    Task<bool> UpdateCommentInTaskAsync(int taskId, string oldComment, string newComment);

    Task<bool> UpdateStatusOfTaskAsync(int taskId, int statusId);

    Task<bool> UpdateTaskAsync(TaskModel task);
}
