using TodoListApp.WebApi.Models;

namespace TodoListApp.ApiClient.Services;
public interface ITodoListApiClientService
{
    Task<HttpResponseMessage> AddEditorAsync(int todoListId, string userId);

    Task<HttpResponseMessage> CreateTodoListAsync(TodoListApiModel todoList);

    Task<HttpResponseMessage> DeleteLodoListsByUserId(string userId);

    Task<HttpResponseMessage> DeleteTodoListAsync(int todoListId);

    Task<TodoListApiModel?> GetTodoListByIdAsync(int todoListId);

    Task<List<TodoListApiModel>?> GetTodoListsByUserIdAsync(string userId);

    Task<HttpResponseMessage> RemoveEditorAsync(int todoListId, string userId);

    Task<HttpResponseMessage> UpdateOwnerAsync(int todoListId, string userId);

    Task<HttpResponseMessage> UpdateTodoListAsync(TodoListApiModel todoList);
}
