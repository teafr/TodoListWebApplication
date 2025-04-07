using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApi.Extensions;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApi.Services;

namespace TodoListApp.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoListsController : ControllerBase
    {
        private readonly ITodoListService todoListService;

        public TodoListsController(ITodoListService todoListService)
        {
            this.todoListService = todoListService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            List<TodoList> todoLists = await this.todoListService.GetTodoListsAsync();
            return this.Ok(todoLists.Select(todoList => todoList.ToTodoListApiModel()));
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(string userId)
        {
            List<TodoList> todoLists = await this.todoListService.GetTodoListsByUserIdAsync(userId);
            return this.Ok(todoLists.Select(todoList => todoList.ToTodoListApiModel()));
        }

        [HttpGet("{todoListId:int}")]
        public async Task<IActionResult> GetById(int todoListId)
        {
            TodoList? todoList = await this.todoListService.GetTodoListByIdAsync(todoListId);
            if (todoList is null)
            {
                return this.NotFound();
            }

            return this.Ok(todoList.ToTodoListApiModel());
        }

        [HttpGet("{todoListId:int}/status/{statusId:int}/tasks")]
        public async Task<IActionResult> GetTasksByTodoListIdAndStatusId(int todoListId, int statusId)
        {
            List<Models.Task> tasks = await this.todoListService.GetTasksByTodoListIdAndStatusIdAsync(todoListId, statusId);
            return this.Ok(tasks.Select(task => task.ToTaskApiModel()));
        }

        [HttpGet("{todoListId:int}/tag/{tag}/tasks")]
        public async Task<IActionResult> GetTasksByTag(int todoListId, string tag)
        {
            List<Models.Task> tasks = await this.todoListService.GetTasksByTodoListIdAndTag(todoListId, tag);
            return this.Ok(tasks.Select(task => task.ToTaskApiModel()));
        }

        [HttpGet("{todoListId:int}/tasks")]
        public async Task<IActionResult> GetTasks(int todoListId)
        {
            List<Models.Task> tasks = await this.todoListService.GetTasksByTodoListIdAsync(todoListId);
            return this.Ok(tasks.Select(task => task.ToTaskApiModel()));
        }

        [HttpGet("{todoListId:int}/user/{userId}/tasks")]
        public async Task<IActionResult> GetTasksByUserId(int todoListId, string userId)
        {
            List<Models.Task> tasks = await this.todoListService.GetTasksByTodoListIdAsync(todoListId);
            return this.Ok(tasks.Where(task => task.AssigneeId == userId).Select(task => task.ToTaskApiModel()));
        }

        [HttpPost]
        public async Task<IActionResult> Create(TodoListApiModel newTodoList)
        {
            TodoList createdTodoList = await this.todoListService.CreateTodoListAsync(new TodoList(newTodoList));
            return this.CreatedAtAction(nameof(this.Create), createdTodoList.ToTodoListApiModel());
        }

        [HttpPost("{todoListId:int}")]
        public async Task<IActionResult> AddTask(int todoListId, TaskApiModel apiTask)
        {
            Models.Task task = new Models.Task(apiTask);
            task.TodoListId = todoListId;

            await this.todoListService.AddTaskAsync(task);
            return this.NoContent();
        }

        [HttpPut]
        public async Task<IActionResult> Update(TodoListApiModel todoListApiModel)
        {
            ArgumentNullException.ThrowIfNull(todoListApiModel);

            TodoList? todoList = await this.todoListService.GetTodoListByIdAsync(todoListApiModel.Id);
            if (todoList is null)
            {
                return this.NotFound();
            }

            await this.todoListService.UpdateTodoListAsync(new TodoList(todoListApiModel));
            return this.NoContent();
        }

        [HttpDelete("{todoListId:int}")]
        public async Task<IActionResult> Delete(int todoListId)
        {
            TodoList? todoList = await this.todoListService.GetTodoListByIdAsync(todoListId);
            if (todoList is null)
            {
                return this.NotFound();
            }

            await this.todoListService.DeleteTodoListByIdAsync(todoListId);
            return this.NoContent();
        }
    }
}
