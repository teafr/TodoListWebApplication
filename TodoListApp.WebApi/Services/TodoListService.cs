using Serilog;
using TodoListApp.Database.Entities;
using TodoListApp.Database.Repositories;
using TodoListApp.WebApi.Extensions;
using TodoListApp.WebApi.Models;

namespace TodoListApp.WebApi.Services;

public class TodoListService : ITodoListService
{
    private readonly IRepository<TodoListEntity> todoListRepository;

    public TodoListService(IRepository<TodoListEntity> todoListRepository)
    {
        this.todoListRepository = todoListRepository;
    }

    public async Task<List<TodoList>> GetTodoListsAsync()
    {
        Log.Debug("Try to get all to-do lists.");

        var todoLists = await this.todoListRepository.GetAsync();
        return todoLists?.Select(list => new TodoList(list))?.ToList() ?? new List<TodoList>();
    }

    public async Task<List<TodoList>> GetTodoListsByUserIdAsync(string userId)
    {
        Log.Debug("Try to get user's to-do lists.");

        var todoLists = await this.todoListRepository.GetAsync();
        return todoLists?.Where(list => list.OwnerId == userId).Select(list => new TodoList(list))?.ToList() ?? new List<TodoList>();
    }

    public async Task<TodoList?> GetTodoListByIdAsync(int id)
    {
        Log.Debug("Try to get to-do list.");

        var todoList = await this.todoListRepository.GetByIdAsync(id);
        if (todoList is not null)
        {
            Log.Information("To-do list was found.");
            return new TodoList(todoList);
        }

        return null;
    }

    public async Task<TodoList> CreateTodoListAsync(TodoList newTodoList)
    {
        Log.Debug("Try to create to-do list.");

        TodoListEntity todoList = await this.todoListRepository.CreateAsync(newTodoList.ToTodoListEntity());
        Log.Information("To-do list was created.");
        return new TodoList(todoList);
    }

    public async System.Threading.Tasks.Task UpdateTodoListAsync(TodoList todoList)
    {
        ArgumentNullException.ThrowIfNull(todoList);
        Log.Debug("Try to update to-do list by id {0}.", todoList.Id);

        TodoListEntity? existingList = await this.todoListRepository.GetByIdAsync(todoList.Id);
        if (existingList is not null)
        {
            existingList!.Title = todoList.Title;
            existingList.Description = todoList.Description;

            this.todoListRepository.Update(existingList);
            Log.Information("To-do list by id {0} was updated.", todoList.Id);
        }
        else
        {
            ThrowIfTodoListNotFound(todoList.Id);
        }
    }

    public async System.Threading.Tasks.Task DeleteTodoListByIdAsync(int id)
    {
        Log.Debug("Try to delete to-do list.");

        TodoListEntity? todoList = await this.todoListRepository.GetByIdAsync(id);
        if (todoList is not null)
        {
            await this.todoListRepository.DeleteByIdAsync(id);
            Log.Information("To-do list was deleted.");
        }
        else
        {
            ThrowIfTodoListNotFound(id);
        }
    }

    public async System.Threading.Tasks.Task DeleteTodoListsByUserIdAsync(string userId)
    {
        Log.Debug("Try to delete to-do lists of user by id {0}.", userId);

        var todoLists = await this.todoListRepository.GetAsync();
        var lists = todoLists?.Where(list => list.OwnerId == userId) ?? Enumerable.Empty<TodoListEntity>();
        foreach (var todoList in lists)
        {
            this.todoListRepository.Delete(todoList);
        }

        Log.Debug("All to-do lists of user by id {0} was deleted.", userId);
    }

    public async Task<List<Models.Task>> GetTasksByTodoListIdAsync(int todoListId)
    {
        Log.Debug("Try to get tasks by to-do list id {0}.", todoListId);

        TodoListEntity? todoList = await this.todoListRepository.GetByIdAsync(todoListId);
        if (todoList is not null)
        {
            return todoList.Tasks.Select(task => new Models.Task(task)).ToList() ?? new List<Models.Task>();
        }
        else
        {
            ThrowIfTodoListNotFound(todoListId);
            return new List<Models.Task>();
        }
    }

    private static void ThrowIfTodoListNotFound(int todoListId)
    {
        Log.Warning("To-do list by id {0} was not found.", todoListId);
        throw new InvalidDataException($"To-do list by id {todoListId} was not found.");
    }
}
