using System.Net.Http.Json;
using TodoListApp.ApiClient.Models;
using TodoListApp.WebApi.Models;

namespace TodoListApp.ApiClient.Services;

public class TodoListApiClientService : IDisposable
{
    private readonly string url = "api/todolists/";
    private readonly HttpClient httpClient;
    private bool disposed;

    public TodoListApiClientService(ApiClientOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        this.httpClient = new HttpClient();
        this.httpClient.BaseAddress = new Uri(options.ApiBaseAdress);
    }

    public async Task<List<TodoListApiModel>?> GetTodoListsAsync()
    {
        return await this.httpClient.GetFromJsonAsync<List<TodoListApiModel>>(this.url);
    }

    public async Task<List<TodoListApiModel>?> GetTodoListsByUserIdAsync(string userId)
    {
        return await this.httpClient.GetFromJsonAsync<List<TodoListApiModel>>(this.url + $"user/{userId}");
    }

    public async Task<TodoListApiModel?> GetTodoListByIdAsync(int todoListId)
    {
        return await this.httpClient.GetFromJsonAsync<TodoListApiModel>(this.url + $"{todoListId}");
    }

    public async Task<List<TaskApiModel>?> GetTasksByTodoListIdAndUserIdAsync(int todoListId, string userId)
    {
        return await this.httpClient.GetFromJsonAsync<List<TaskApiModel>>(this.url + $"{todoListId}/user/{userId}/tasks");
    }

    public async Task<List<TaskApiModel>?> GetTasksByTodoListIdAsync(int todoListId)
    {
        return await this.httpClient.GetFromJsonAsync<List<TaskApiModel>>(this.url + $"{todoListId}/tasks");
    }

    public async Task<List<TaskApiModel>?> GetTasksByTodoListIdAndTagAsync(int todoListId, string tag)
    {
        return await this.httpClient.GetFromJsonAsync<List<TaskApiModel>>(this.url + $"{todoListId}/tag/{tag}/tasks");
    }

    public async Task<List<TaskApiModel>?> GetTasksByTodoListIdAndStatusIdAsync(int todoListId, int statusId)
    {
        return await this.httpClient.GetFromJsonAsync<List<TaskApiModel>>(this.url + $"{todoListId}/status/{statusId}/tasks");
    }

    public async Task CreateTodoListAsync(TodoListApiModel todoList)
    {
        _ = await this.httpClient.PostAsJsonAsync(this.url, todoList);
    }

    public async Task AddTaskToTodoListAsync(int todoListId, TaskApiModel task)
    {
        _ = await this.httpClient.PostAsJsonAsync(this.url + $"{todoListId}", task);
    }

    public async Task UpdateTodoListAsync(TodoListApiModel todoList)
    {
        _ = await this.httpClient.PutAsJsonAsync(this.url, todoList);
    }

    public async Task DeleteTodoListAsync(int todoListId)
    {
        _ = await this.httpClient.DeleteAsync(new Uri(this.url + $"{todoListId}"));
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
