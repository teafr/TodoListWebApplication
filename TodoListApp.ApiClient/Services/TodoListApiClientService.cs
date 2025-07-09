using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using TodoListApp.WebApi.Models;

namespace TodoListApp.ApiClient.Services;

public class TodoListApiClientService : IDisposable, ITodoListApiClientService
{
    private readonly string url = "api/todolists/";
    private readonly HttpClient httpClient;
    private readonly JsonSerializerOptions options = new JsonSerializerOptions()
    {
        PropertyNameCaseInsensitive = true,
    };

    private bool disposed;

    public TodoListApiClientService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<List<TodoListApiModel>?> GetTodoListsByUserIdAsync(string userId)
    {
        return await this.httpClient.GetFromJsonAsync<List<TodoListApiModel>>(this.url + $"user/{userId}");
    }

    public async Task<TodoListApiModel?> GetTodoListByIdAsync(int todoListId)
    {
        HttpResponseMessage response = await this.httpClient.GetAsync(new Uri(this.httpClient.BaseAddress + this.url + $"{todoListId}"));

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        _ = response.EnsureSuccessStatusCode();
        return await JsonSerializer.DeserializeAsync<TodoListApiModel>(await response.Content.ReadAsStreamAsync(), this.options);
    }

    public async Task<HttpResponseMessage> CreateTodoListAsync(TodoListApiModel todoList)
    {
        return await this.httpClient.PostAsJsonAsync(this.url, todoList);
    }

    public async Task<HttpResponseMessage> AddEditorAsync(int todoListId, string userId)
    {
        return await this.httpClient.PostAsJsonAsync(new Uri(this.httpClient.BaseAddress + this.url + $"{todoListId}/editors"), userId);
    }

    public async Task<HttpResponseMessage> UpdateTodoListAsync(TodoListApiModel todoList)
    {
        return await this.httpClient.PutAsJsonAsync(this.url, todoList);
    }

    public async Task<HttpResponseMessage> DeleteTodoListAsync(int todoListId)
    {
        return await this.httpClient.DeleteAsync(new Uri(this.httpClient.BaseAddress + this.url + $"{todoListId}"));
    }

    public async Task<HttpResponseMessage> DeleteLodoListsByUserId(string userId)
    {
        return await this.httpClient.DeleteAsync(new Uri(this.httpClient.BaseAddress + this.url + $"user/{userId}"));
    }

    public async Task<HttpResponseMessage> RemoveEditorAsync(int todoListId, string userId)
    {
        return await this.httpClient.DeleteAsync(new Uri(this.httpClient.BaseAddress + this.url + $"{todoListId}/editors/{userId}"));
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
