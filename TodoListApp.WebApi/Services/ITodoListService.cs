using TodoListApp.WebApi.Models;

namespace TodoListApp.WebApi.Services;

public interface ITodoListService
{
    Task<TodoList> CreateTodoListAsync(TodoList newTodoList);

    System.Threading.Tasks.Task DeleteTodoListByIdAsync(int id);

    System.Threading.Tasks.Task DeleteTodoListsByUserIdAsync(string userId);

    Task<List<Models.Task>> GetTasksByTodoListIdAsync(int todoListId);

    Task<TodoList?> GetTodoListByIdAsync(int id);

    Task<List<TodoList>> GetTodoListsAsync();

    Task<List<TodoList>> GetTodoListsByUserIdAsync(string userId);

    System.Threading.Tasks.Task UpdateTodoListAsync(TodoList todoList);
}
