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
        var todoLists = await this.todoListRepository.GetAsync();
        return todoLists?.Select(list => new TodoList(list))?.ToList() ?? new List<TodoList>();
    }

    public async Task<List<TodoList>> GetTodoListsByUserIdAsync(string userId)
    {
        var todoLists = await this.todoListRepository.GetAsync();
        return todoLists?.Where(list => list.OwnerId == userId).Select(list => new TodoList(list))?.ToList() ?? new List<TodoList>();
    }

    public async Task<TodoList?> GetTodoListByIdAsync(int id)
    {
        var todoList = await this.todoListRepository.GetByIdAsync(id);
        return todoList is null ? null : new TodoList(todoList);
    }

    public async Task<TodoList> CreateTodoListAsync(TodoList newTodoList)
    {
        TodoListEntity todoList = await this.todoListRepository.CreateAsync(newTodoList.ToTodoListEntity());
        return new TodoList(todoList);
    }

    public async System.Threading.Tasks.Task UpdateTodoListAsync(TodoList todoList)
    {
        if (todoList is not null)
        {
            TodoListEntity? existingList = await this.todoListRepository.GetByIdAsync(todoList.Id);
            if (existingList is not null)
            {
                existingList!.Title = todoList.Title;
                existingList.Description = todoList.Description;

                this.todoListRepository.Update(existingList);
            }
        }
    }

    public async System.Threading.Tasks.Task DeleteTodoListByIdAsync(int id)
    {
        await this.todoListRepository.DeleteByIdAsync(id);
    }

    public async System.Threading.Tasks.Task DeleteTodoListsByUserIdAsync(string userId)
    {
        var todoLists = await this.todoListRepository.GetAsync();
        var lists = todoLists?.Where(list => list.OwnerId == userId) ?? Enumerable.Empty<TodoListEntity>();
        foreach (var todoList in lists)
        {
            this.todoListRepository.Delete(todoList);
        }
    }

    public async Task<List<Models.Task>> GetTasksByTodoListIdAsync(int todoListId)
    {
        var todoList = await this.todoListRepository.GetByIdAsync(todoListId);
        return todoList?.Tasks.Select(task => new Models.Task(task)).ToList() ?? new List<Models.Task>();
    }
}
