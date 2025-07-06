using TodoListApp.WebApp.Models;

namespace TodoListApp.WebApp.Services;
public interface ITodoListWebApiService
{
    Task<HttpResponseMessage> AddEditorAsync(int todoListId, string editorId);

    Task<HttpResponseMessage> CreateTodoListAsync(TodoListModel todoList);

    Task<HttpResponseMessage> DeleteListsByUserIdAsync(string userId);

    Task<HttpResponseMessage> DeleteTodoListAsync(int todoListId);

    Task<TodoListModel?> GetTodoListByIdAsync(int id);

    Task<List<TodoListModel>> GetTodoListsByUserIdAsync(string userId);

    Task<HttpResponseMessage> RemoveEditorAsync(int todoListId, string editorId);

    Task<HttpResponseMessage> UpdateTodoListAsync(TodoListModel todoList);
}
