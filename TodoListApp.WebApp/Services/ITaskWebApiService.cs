using TodoListApp.WebApp.Models;

namespace TodoListApp.WebApp.Services;
public interface ITaskWebApiService
{
    Task<HttpResponseMessage> AddCommentAsync(int taskId, string comment);

    Task<HttpResponseMessage> AddTagAsync(int taskId, string tag);

    Task<HttpResponseMessage> CreateTaskAsync(TaskModel task);

    Task<HttpResponseMessage> DeleteTaskAsync(int taskId);

    Task<List<string>> GetTagsByUserIdAsync(string userId);

    Task<TaskModel?> GetTaskByIdAsync(int taskId);

    Task<List<TaskModel>> GetTasksByUserIdAsync(string userId);

    Task<HttpResponseMessage> RemoveCommentFromTaskAsync(int taskId, string comment);

    Task<HttpResponseMessage> RemoveTagFromTaskAsync(int taskId, string tag);

    Task<HttpResponseMessage> UpdateAssigneeAsync(int taskId, string assigneeId);

    Task<HttpResponseMessage> UpdateCommentInTaskAsync(int taskId, string oldComment, string newComment);

    Task<HttpResponseMessage> UpdateStatusOfTaskAsync(int taskId, int statusId);

    Task<HttpResponseMessage> UpdateTaskAsync(TaskModel task);
}
