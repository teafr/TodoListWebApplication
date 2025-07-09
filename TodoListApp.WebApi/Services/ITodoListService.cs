using TodoListApp.WebApi.Models;

namespace TodoListApp.WebApi.Services;

public interface ITodoListService
{
    Task<TodoList?> GetTodoListByIdAsync(int id);

    Task<List<TodoList>> GetTodoListsAsync();

    Task<List<TodoList>> GetTodoListsByUserIdAsync(string userId);

    Task<TodoList> CreateTodoListAsync(TodoList newTodoList);

    System.Threading.Tasks.Task UpdateTodoListAsync(TodoList todoList);

    System.Threading.Tasks.Task UpdateEditorsAsync(int todoListId, ICollection<string> editors);

    System.Threading.Tasks.Task DeleteTodoListByIdAsync(int id);
}
