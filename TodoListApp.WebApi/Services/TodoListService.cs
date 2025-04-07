using TodoListApp.WebApi.Entities;
using TodoListApp.WebApi.Extensions;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApi.Repositories;

namespace TodoListApp.WebApi.Services;

internal class TodoListService : ITodoListService
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
        ArgumentNullException.ThrowIfNull(newTodoList);

        TodoListEntity todoList = await this.todoListRepository.CreateAsync(newTodoList.ToTodoListEntity());
        return new TodoList(todoList);
    }

    public async System.Threading.Tasks.Task UpdateTodoListAsync(TodoList todoList)
    {
        ArgumentNullException.ThrowIfNull(todoList);

        TodoListEntity? existingList = await this.todoListRepository.GetByIdAsync(todoList.Id);
        if (existingList is not null)
        {
            existingList!.Title = todoList.Title;
            existingList.Description = todoList.Description;
            existingList.OwnerId = todoList.OwnerId;

            await this.todoListRepository.UpdateAsync(existingList);
        }
    }

    public async System.Threading.Tasks.Task DeleteTodoListByIdAsync(int id)
    {
        var todoList = await this.todoListRepository.GetByIdAsync(id);
        if (todoList is not null)
        {
            await this.todoListRepository.DeleteAsync(todoList);
        }
    }

    public async Task<List<Models.Task>> GetTasksByTodoListIdAsync(int todoListId)
    {
        var todoList = await this.todoListRepository.GetByIdAsync(todoListId);
        return todoList?.Tasks.Select(task => new Models.Task(task)).ToList() ?? new List<Models.Task>();
    }

    public async Task<List<Models.Task>> GetTasksByAssigneeIdAsync(string assigneeId)
    {
        List<TodoListEntity>? todoLists = await this.todoListRepository.GetAsync();
        return todoLists?.SelectMany(list => list.Tasks)
                         .Where(task => task.AssigneeId == assigneeId)
                         .Select(task => new Models.Task(task))?.ToList() ?? new List<Models.Task>();
    }

    public async Task<List<Models.Task>> GetTasksByTodoListIdAndStatusIdAsync(int todoListId, int statusId)
    {
        var todoList = await this.todoListRepository.GetByIdAsync(todoListId);
        return todoList?.Tasks.Where(task => task.StatusId == statusId)
                              .Select(task => new Models.Task(task)).ToList() ?? new List<Models.Task>();
    }

    public async Task<List<Models.Task>> GetTasksByTodoListIdAndTag(int todoListId, string tag)
    {
        var todoList = await this.todoListRepository.GetByIdAsync(todoListId);
        return todoList?.Tasks.Where(task => task?.Tags?.Any(t => t == tag) ?? false)
                              .Select(task => new Models.Task(task)).ToList() ?? new List<Models.Task>();
    }

    public async System.Threading.Tasks.Task AddTaskAsync(Models.Task newTask)
    {
        ArgumentNullException.ThrowIfNull(newTask);

        TodoListEntity? existingTodoList = await this.todoListRepository.GetByIdAsync(newTask.TodoListId);
        existingTodoList!.Tasks.Add(newTask.ToTaskEntity());
        await this.todoListRepository.UpdateAsync(existingTodoList);
    }
}
