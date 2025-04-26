using TodoListApp.WebApp.Models;

namespace TodoListApp.WebApp.Services;
public interface ITodoListWebApiService
{
    Task<TodoListModel?> GetTodoListByIdAsync(int id);

    Task<List<TaskModel>> GetTasksByTodoListIdAsync(int todoListId);

    Task<List<TodoListModel>> GetTodoListsByUserIdAsync(string userId);

    Task CreateTodoListAsync(TodoListModel todoList);

    Task UpdateTodoListAsync(TodoListModel todoList);

    Task DeleteListsByUserIdAsync(string userId);

    Task DeleteTodoListAsync(int todoListId);
}
