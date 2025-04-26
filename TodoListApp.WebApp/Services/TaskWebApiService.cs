using TodoListApp.ApiClient.Services;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApp.Extensions;
using TodoListApp.WebApp.Models;

namespace TodoListApp.WebApp.Services;

public class TaskWebApiService : ITaskWebApiService
{
    private readonly TaskApiClientService taskApiClient;

    public TaskWebApiService(TaskApiClientService taskApiClient)
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

    public async System.Threading.Tasks.Task CreateTaskAsync(TaskModel task)
    {
        await this.taskApiClient.CreateTaskAsync(task.ToTaskApiModel());
    }

    public async System.Threading.Tasks.Task AddTagAsync(int taskId, string tag)
    {
        await this.taskApiClient.AddTagAsync(taskId, tag);
    }

    public async System.Threading.Tasks.Task AddCommentAsync(int taskId, string comment)
    {
        await this.taskApiClient.AddCommentAsync(taskId, comment);
    }

    public async System.Threading.Tasks.Task UpdateTaskAsync(TaskModel task)
    {
        await this.taskApiClient.UpdateTaskAsync(task.ToTaskApiModel());
    }

    public async System.Threading.Tasks.Task UpdateStatusOfTaskAsync(int taskId, int statusId)
    {
        await this.taskApiClient.UpdateStatusOfTaskAsync(taskId, statusId);
    }

    public async System.Threading.Tasks.Task UpdateAssigneeAsync(int taskId, string assigneeId)
    {
        await this.taskApiClient.UpdateAssigneeAsync(taskId, assigneeId);
    }

    public async System.Threading.Tasks.Task UpdateCommentInTaskAsync(int taskId, string oldComment, string newComment)
    {
        await this.taskApiClient.UpdateCommentInTaskAsync(taskId, oldComment, newComment);
    }

    public async System.Threading.Tasks.Task DeleteTaskAsync(int taskId)
    {
        await this.taskApiClient.DeleteTaskAsync(taskId);
    }

    public async System.Threading.Tasks.Task RemoveTagFromTaskAsync(int taskId, string tag)
    {
        await this.taskApiClient.RemoveTagFromTaskAsync(taskId, tag);
    }

    public async System.Threading.Tasks.Task RemoveCommentFromTaskAsync(int taskId, string comment)
    {
        await this.taskApiClient.RemoveCommentFromTaskAsync(taskId, comment);
    }
}
