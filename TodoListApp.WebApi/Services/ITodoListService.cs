using TodoListApp.WebApi.Models;

namespace TodoListApp.WebApi.Services;
public interface ITodoListService
{
    System.Threading.Tasks.Task AddTaskAsync(Models.Task newTask);

    Task<TodoList> CreateTodoListAsync(TodoList newTodoList);

    System.Threading.Tasks.Task DeleteTodoListByIdAsync(int id);

    Task<List<Models.Task>> GetTasksByAssigneeIdAsync(string assigneeId);

    Task<List<Models.Task>> GetTasksByTodoListIdAndStatusIdAsync(int todoListId, int statusId);

    Task<List<Models.Task>> GetTasksByTodoListIdAndTag(int todoListId, string tag);

    Task<List<Models.Task>> GetTasksByTodoListIdAsync(int todoListId);

    Task<TodoList?> GetTodoListByIdAsync(int id);

    Task<List<TodoList>> GetTodoListsAsync();

    Task<List<TodoList>> GetTodoListsByUserIdAsync(string userId);

    System.Threading.Tasks.Task UpdateTodoListAsync(TodoList todoList);

}
