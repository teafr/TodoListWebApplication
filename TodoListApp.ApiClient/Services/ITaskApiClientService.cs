using TodoListApp.WebApi.Models;

namespace TodoListApp.ApiClient.Services;
public interface ITaskApiClientService
{
    Task<bool> AddCommentAsync(int taskId, string comment);
    Task<bool> AddTagAsync(int taskId, string tag);
    Task<bool> CreateTaskAsync(TaskApiModel task);
    Task<bool> DeleteTaskAsync(int taskId);
    Task<List<string>?> GetTagsByUserIdAsync(string userId);
    Task<TaskApiModel?> GetTaskByIdAsync(int taskId);
    Task<List<TaskApiModel>?> GetTasksByUserIdAsync(string userId);
    Task<bool> RemoveCommentFromTaskAsync(int taskId, string comment);
    Task<bool> RemoveTagFromTaskAsync(int taskId, string tag);
    Task<bool> UpdateAssigneeAsync(int taskId, string assigneeId);
    Task<bool> UpdateCommentInTaskAsync(int taskId, string oldcomment, string newComment);
    Task<bool> UpdateStatusOfTaskAsync(int taskId, int statusId);
    Task<bool> UpdateTaskAsync(TaskApiModel task);
}