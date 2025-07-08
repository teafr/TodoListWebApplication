using System.Text.Json;
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
        return todoLists?.Where(list => list.OwnerId == userId || (JsonSerializer.Deserialize<List<string>>(list.Editors ?? string.Empty)?
                         .Contains(userId) ?? false)).Select(list => new TodoList(list)).ToList() ?? new List<TodoList>();
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

    public async System.Threading.Tasks.Task UpdateOwner(int todoListId, string ownerId)
    {
        Log.Debug("Try to update owner of to-do list by id {0}.", todoListId);
        TodoListEntity? existingList = await this.todoListRepository.GetByIdAsync(todoListId);
        if (existingList is not null)
        {
            existingList.OwnerId = ownerId;
            this.todoListRepository.Update(existingList);
            Log.Information("Owner of to-do list by id {0} was updated.", todoListId);
        }
        else
        {
            ThrowIfTodoListNotFound(todoListId);
        }
    }

    public async System.Threading.Tasks.Task UpdateEditors(int todoListId, ICollection<string> editors)
    {
        ArgumentNullException.ThrowIfNull(editors);
        Log.Debug("Try to update editors of to-do list by id {0}.", todoListId);

        TodoListEntity? existingList = await this.todoListRepository.GetByIdAsync(todoListId);
        if (existingList is not null)
        {
            existingList.Editors = JsonSerializer.Serialize(editors);
            this.todoListRepository.Update(existingList);
            Log.Information("Editors of to-do list by id {0} was updated.", todoListId);
        }
        else
        {
            ThrowIfTodoListNotFound(todoListId);
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

    private static void ThrowIfTodoListNotFound(int todoListId)
    {
        Log.Warning("To-do list by id {0} was not found.", todoListId);
        throw new InvalidDataException($"To-do list by id {todoListId} was not found.");
    }
}
