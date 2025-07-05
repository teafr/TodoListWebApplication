using TodoListApp.WebApi.Models;

namespace TodoListApp.ApiClient.Services;
public interface ITodoListApiClientService
{
    Task<bool> AddEditorAsync(int todoListId, string userId);
    Task<bool> CreateTodoListAsync(TodoListApiModel todoList);
    Task<bool> DeleteLodoListsByUserId(string userId);
    Task<bool> DeleteTodoListAsync(int todoListId);
    Task<TodoListApiModel?> GetTodoListByIdAsync(int todoListId);
    Task<List<TodoListApiModel>?> GetTodoListsByUserIdAsync(string userId);
    Task<bool> RemoveEditorAsync(int todoListId, string userId);
    Task<bool> UpdateOwnerAsync(int todoListId, string userId);
    Task<bool> UpdateTodoListAsync(TodoListApiModel todoList);
}