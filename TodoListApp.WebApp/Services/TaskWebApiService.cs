using TodoListApp.ApiClient.Services;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApp.Extensions;
using TodoListApp.WebApp.Models;

namespace TodoListApp.WebApp.Services;

public class TaskWebApiService : ITaskWebApiService
{
    private readonly ITaskApiClientService taskApiClient;

    public TaskWebApiService(ITaskApiClientService taskApiClient)
    {
        this.taskApiClient = taskApiClient;
    }

    public async Task<List<TaskModel>> GetTasksByUserIdAsync(string userId)
    {
        List<TaskApiModel>? tasks = await this.taskApiClient.GetTasksByUserIdAsync(userId);
        return tasks?.Select(task => new TaskModel(task)).ToList() ?? new List<TaskModel>();
    }

    public async Task<TaskModel?> GetTaskByIdAsync(int taskId)
    {
        TaskApiModel? task = await this.taskApiClient.GetTaskByIdAsync(taskId);
        return task is null ? null : new TaskModel(task);
    }

    public async Task<List<string>> GetTagsByUserIdAsync(string userId)
    {
        return (await this.taskApiClient.GetTagsByUserIdAsync(userId)) ?? new List<string>();
    }

    public async Task<bool> CreateTaskAsync(TaskModel task)
    {
        return await this.taskApiClient.CreateTaskAsync(task.ToTaskApiModel());
    }

    public async Task<bool> AddTagAsync(int taskId, string tag)
    {
        return await this.taskApiClient.AddTagAsync(taskId, tag);
    }

    public async Task<bool> AddCommentAsync(int taskId, string comment)
    {
        return await this.taskApiClient.AddCommentAsync(taskId, comment);
    }

    public async Task<bool> UpdateTaskAsync(TaskModel task)
    {
        return await this.taskApiClient.UpdateTaskAsync(task.ToTaskApiModel());
    }

    public async Task<bool> UpdateStatusOfTaskAsync(int taskId, int statusId)
    {
        return await this.taskApiClient.UpdateStatusOfTaskAsync(taskId, statusId);
    }

    public async Task<bool> UpdateAssigneeAsync(int taskId, string assigneeId)
    {
        return await this.taskApiClient.UpdateAssigneeAsync(taskId, assigneeId);
    }

    public async Task<bool> UpdateCommentInTaskAsync(int taskId, string oldComment, string newComment)
    {
        return await this.taskApiClient.UpdateCommentInTaskAsync(taskId, oldComment, newComment);
    }

    public async Task<bool> DeleteTaskAsync(int taskId)
    {
        return await this.taskApiClient.DeleteTaskAsync(taskId);
    }

    public async Task<bool> RemoveTagFromTaskAsync(int taskId, string tag)
    {
        return await this.taskApiClient.RemoveTagFromTaskAsync(taskId, tag);
    }

    public async Task<bool> RemoveCommentFromTaskAsync(int taskId, string comment)
    {
        return await this.taskApiClient.RemoveCommentFromTaskAsync(taskId, comment);
    }
}
