using TodoListApp.ApiClient.Services;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApp.Extensions;
using TodoListApp.WebApp.Models;

namespace TodoListApp.WebApp.Services;

public class TodoListWebApiService : ITodoListWebApiService
{
    private readonly TodoListApiClientService todoListApiClient;

    public TodoListWebApiService(TodoListApiClientService todoListApiClient)
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

    public async Task<List<TaskModel>> GetTasksByTodoListIdAsync(int todoListId)
    {
        List<TaskApiModel>? tasks = await this.todoListApiClient.GetTasksByTodoListIdAsync(todoListId);
        return tasks?.Select(task => new TaskModel(task)).ToList() ?? new List<TaskModel>();
    }

    public async System.Threading.Tasks.Task CreateTodoListAsync(TodoListModel todoList)
    {
        await this.todoListApiClient.CreateTodoListAsync(todoList.ToTodoListApiModel());
    }

    public async System.Threading.Tasks.Task UpdateTodoListAsync(TodoListModel todoList)
    {
        await this.todoListApiClient.UpdateTodoListAsync(todoList.ToTodoListApiModel());
    }

    public async System.Threading.Tasks.Task DeleteTodoListAsync(int todoListId)
    {
        await this.todoListApiClient.DeleteTodoListAsync(todoListId);
    }

    public async System.Threading.Tasks.Task DeleteListsByUserIdAsync(string userId)
    {
        await this.todoListApiClient.DeleteLodoListsByUserId(userId);
    }
}
