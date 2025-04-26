using TodoListApp.WebApp.Models;

namespace TodoListApp.WebApp.Services;
public interface ITodoListWebApiService
{
    Task<TodoListModel?> GetTodoListByIdAsync(int id);

    Task<List<TaskModel>> GetTasksByTodoListIdAsync(int todoListId);

    Task<List<TodoListModel>> GetTodoListsByUserIdAsync(string userId);

    System.Threading.Tasks.Task CreateTodoListAsync(TodoListModel todoList);

    System.Threading.Tasks.Task UpdateTodoListAsync(TodoListModel todoList);

    System.Threading.Tasks.Task DeleteListsByUserIdAsync(string userId);

    System.Threading.Tasks.Task DeleteTodoListAsync(int todoListId);
}
