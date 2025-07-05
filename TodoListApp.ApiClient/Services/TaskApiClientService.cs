using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using TodoListApp.WebApi.Models;

namespace TodoListApp.ApiClient.Services;

public class TaskApiClientService : IDisposable, ITaskApiClientService
{
    private readonly string url = "api/tasks/";
    private readonly HttpClient httpClient;
    private readonly JsonSerializerOptions options = new JsonSerializerOptions()
    {
        PropertyNameCaseInsensitive = true,
    };

    private bool disposed;

    public TaskApiClientService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<TaskApiModel?> GetTaskByIdAsync(int taskId)
    {
        HttpResponseMessage response = await this.httpClient.GetAsync(new Uri(this.httpClient.BaseAddress + this.url + $"{taskId}"));

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        _ = response.EnsureSuccessStatusCode();

        var stream = await response.Content.ReadAsStreamAsync();
        return await JsonSerializer.DeserializeAsync<TaskApiModel>(stream, this.options);
    }

    public async Task<List<TaskApiModel>?> GetTasksByUserIdAsync(string userId)
    {
        return await this.httpClient.GetFromJsonAsync<List<TaskApiModel>>(this.url + $"user/{userId}");
    }

    public async Task<List<string>?> GetTagsByUserIdAsync(string userId)
    {
        return await this.httpClient.GetFromJsonAsync<List<string>>(this.url + $"{userId}/tags");
    }

    public async Task<bool> CreateTaskAsync(TaskApiModel task)
    {
        return (await this.httpClient.PostAsJsonAsync(this.url, task)).IsSuccessStatusCode;
    }

    public async Task<bool> AddTagAsync(int taskId, string tag)
    {
        return (await this.httpClient.PostAsync(new Uri(this.httpClient.BaseAddress + this.url + $"{taskId}/tag/{tag}"), null)).IsSuccessStatusCode;
    }

    public async Task<bool> AddCommentAsync(int taskId, string comment)
    {
        return (await this.httpClient.PostAsync(new Uri(this.httpClient.BaseAddress + this.url + $"{taskId}/comment/{comment}"), null)).IsSuccessStatusCode;
    }

    public async Task<bool> UpdateTaskAsync(TaskApiModel task)
    {
        return (await this.httpClient.PutAsJsonAsync(this.url, task)).IsSuccessStatusCode;
    }

    public async Task<bool> UpdateStatusOfTaskAsync(int taskId, int statusId)
    {
        return (await this.httpClient.PutAsync(new Uri(this.httpClient.BaseAddress + this.url + $"{taskId}/status/{statusId}"), null)).IsSuccessStatusCode;
    }

    public async Task<bool> UpdateAssigneeAsync(int taskId, string assigneeId)
    {
        return (await this.httpClient.PutAsync(new Uri(this.httpClient.BaseAddress + this.url + $"{taskId}/user/{assigneeId}"), null)).IsSuccessStatusCode;
    }

    public async Task<bool> UpdateCommentInTaskAsync(int taskId, string oldcomment, string newComment)
    {
        return (await this.httpClient.PutAsync(new Uri(this.httpClient.BaseAddress + this.url + $"{taskId}/comment/{oldcomment}/{newComment}"), null)).IsSuccessStatusCode;
    }

    public async Task<bool> DeleteTaskAsync(int taskId)
    {
        return (await this.httpClient.DeleteAsync(new Uri(this.httpClient.BaseAddress + this.url + $"{taskId}"))).IsSuccessStatusCode;
    }

    public async Task<bool> RemoveTagFromTaskAsync(int taskId, string tag)
    {
        return (await this.httpClient.DeleteAsync(new Uri(this.httpClient.BaseAddress + this.url + $"{taskId}/tag/{tag}"))).IsSuccessStatusCode;
    }

    public async Task<bool> RemoveCommentFromTaskAsync(int taskId, string comment)
    {
        return (await this.httpClient.DeleteAsync(new Uri(this.httpClient.BaseAddress + this.url + $"{taskId}/comment/{comment}"))).IsSuccessStatusCode;
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                this.httpClient.Dispose();
            }

            this.disposed = true;
        }
    }
}
