using TodoListApp.WebApp.Models;

namespace TodoListApp.WebApp.Services;
public interface ITodoListWebApiService
{
    Task<bool> AddEditorAsync(int todoListId, string editorId);

    Task<bool> CreateTodoListAsync(TodoListModel todoList);

    Task<bool> DeleteListsByUserIdAsync(string userId);

    Task<bool> DeleteTodoListAsync(int todoListId);

    Task<TodoListModel?> GetTodoListByIdAsync(int id);

    Task<List<TodoListModel>> GetTodoListsByUserIdAsync(string userId);

    Task<bool> RemoveEditorAsync(int todoListId, string editorId);

    Task<bool> UpdateTodoListAsync(TodoListModel todoList);
}
