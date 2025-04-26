using TodoListApp.WebApp.Models;

namespace TodoListApp.WebApp.Services;
public interface ITaskWebApiService
{
    Task<TaskModel?> GetTaskByIdAsync(int taskId);

    Task<List<TaskModel>> GetTasksByUserIdAsync(string userId);

    Task<List<string>> GetTagsByUserIdAsync(string userId);

    System.Threading.Tasks.Task AddCommentAsync(int taskId, string comment);

    System.Threading.Tasks.Task AddTagAsync(int taskId, string tag);

    System.Threading.Tasks.Task CreateTaskAsync(TaskModel task);

    System.Threading.Tasks.Task UpdateCommentInTaskAsync(int taskId, string oldComment, string newComment);

    System.Threading.Tasks.Task UpdateStatusOfTaskAsync(int taskId, int statusId);

    System.Threading.Tasks.Task UpdateAssigneeAsync(int taskId, string assigneeId);

    System.Threading.Tasks.Task UpdateTaskAsync(TaskModel task);

    System.Threading.Tasks.Task DeleteTaskAsync(int taskId);

    System.Threading.Tasks.Task RemoveCommentFromTaskAsync(int taskId, string comment);

    System.Threading.Tasks.Task RemoveTagFromTaskAsync(int taskId, string tag);
}
