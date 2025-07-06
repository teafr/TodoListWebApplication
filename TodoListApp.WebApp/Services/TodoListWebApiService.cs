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

    public async Task<bool> CreateTodoListAsync(TodoListModel todoList)
    {
        return await this.todoListApiClient.CreateTodoListAsync(todoList.ToTodoListApiModel());
    }

    public async Task<bool> AddEditorAsync(int todoListId, string editorId)
    {
        return await this.todoListApiClient.AddEditorAsync(todoListId, editorId);
    }

    public async Task<bool> UpdateTodoListAsync(TodoListModel todoList)
    {
        return await this.todoListApiClient.UpdateTodoListAsync(todoList.ToTodoListApiModel());
    }

    public async Task<bool> DeleteTodoListAsync(int todoListId)
    {
        return await this.todoListApiClient.DeleteTodoListAsync(todoListId);
    }

    public async Task<bool> DeleteListsByUserIdAsync(string userId)
    {
        return await this.todoListApiClient.DeleteLodoListsByUserId(userId);
    }

    public async Task<bool> RemoveEditorAsync(int todoListId, string editorId)
    {
        return await this.todoListApiClient.RemoveEditorAsync(todoListId, editorId);
    }
}
