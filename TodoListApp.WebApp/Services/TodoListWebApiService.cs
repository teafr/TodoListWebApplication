using TodoListApp.ApiClient.Services;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApp.Extensions;
using TodoListApp.WebApp.Models;

namespace TodoListApp.WebApp.Services;

public class TodoListWebApiService : ITodoListWebApiService
{
    private readonly ITodoListApiClientService todoListApiClient;

    public TodoListWebApiService(ITodoListApiClientService todoListApiClient)
    {
        this.todoListApiClient = todoListApiClient;
    }

    public async Task<List<TodoListModel>> GetTodoListsByUserIdAsync(string userId)
    {
        List<TodoListApiModel>? todoLists = await this.todoListApiClient.GetTodoListsByUserIdAsync(userId);
        return todoLists?.Select(list => new TodoListModel(list)).ToList() ?? new List<TodoListModel>();
    }

    public async Task<TodoListModel?> GetTodoListByIdAsync(int id)
    {
        TodoListApiModel? todoList = await this.todoListApiClient.GetTodoListByIdAsync(id);
        return todoList is null ? null : new TodoListModel(todoList);
    }

    public async Task CreateTodoListAsync(TodoListModel todoList)
    {
        await this.todoListApiClient.CreateTodoListAsync(todoList.ToTodoListApiModel());
    }

    public async Task AddEditorAsync(int todoListId, string editorId)
    {
        await this.todoListApiClient.AddEditorAsync(todoListId, editorId);
    }

    public async Task UpdateTodoListAsync(TodoListModel todoList)
    {
        await this.todoListApiClient.UpdateTodoListAsync(todoList.ToTodoListApiModel());
    }

    public async Task DeleteTodoListAsync(int todoListId)
    {
        await this.todoListApiClient.DeleteTodoListAsync(todoListId);
    }

    public async Task DeleteListsByUserIdAsync(string userId)
    {
        await this.todoListApiClient.DeleteLodoListsByUserId(userId);
    }

    public async Task RemoveEditorAsync(int todoListId, string editorId)
    {
        await this.todoListApiClient.RemoveEditorAsync(todoListId, editorId);
    }
}
