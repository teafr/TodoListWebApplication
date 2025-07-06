using TodoListApp.WebApi.Models;

namespace TodoListApp.ApiClient.Services;
public interface ITaskApiClientService
{
    Task<HttpResponseMessage> AddCommentAsync(int taskId, string comment);

    Task<HttpResponseMessage> AddTagAsync(int taskId, string tag);

    Task<HttpResponseMessage> CreateTaskAsync(TaskApiModel task);

    Task<HttpResponseMessage> DeleteTaskAsync(int taskId);

    Task<List<string>?> GetTagsByUserIdAsync(string userId);

    Task<TaskApiModel?> GetTaskByIdAsync(int taskId);

    Task<List<TaskApiModel>?> GetTasksByUserIdAsync(string userId);

    Task<HttpResponseMessage> RemoveCommentFromTaskAsync(int taskId, string comment);

    Task<HttpResponseMessage> RemoveTagFromTaskAsync(int taskId, string tag);

    Task<HttpResponseMessage> UpdateAssigneeAsync(int taskId, string assigneeId);

    Task<HttpResponseMessage> UpdateCommentInTaskAsync(int taskId, string oldcomment, string newComment);

    Task<HttpResponseMessage> UpdateStatusOfTaskAsync(int taskId, int statusId);

    Task<HttpResponseMessage> UpdateTaskAsync(TaskApiModel task);
}
