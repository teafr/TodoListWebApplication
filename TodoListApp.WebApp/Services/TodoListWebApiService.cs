using Newtonsoft.Json;
using TodoListApp.WebApi.Models;

namespace TodoListApp.WebApp.Services;

public class TodoListWebApiService
{
    private readonly HttpClient httpClient;

    public TodoListWebApiService(HttpClient httpClient)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        this.httpClient = httpClient;
    }

    public async Task<TodoListApiModel?> GetTodoListAsync(int id)
    {
        var response = await this.httpClient.GetAsync(new Uri($"api/todolists/{id}"));
        _ = response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<TodoListApiModel>(content);
    }
}
