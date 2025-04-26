using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApi.Extensions;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApi.Services;

namespace TodoListApp.WebApi.Controllers
{
    [Authorize]
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> Get()
        {
            List<TodoList> todoLists = await this.todoListService.GetTodoListsAsync();
            return this.Ok(todoLists.Select(todoList => todoList.ToTodoListApiModel()));
        }

        [HttpGet("user/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> GetByUserId(string userId)
        {
            List<TodoList> todoLists = await this.todoListService.GetTodoListsByUserIdAsync(userId);
            return this.Ok(todoLists.Select(todoList => todoList.ToTodoListApiModel()));
        }

        [HttpGet("{todoListId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> GetById(int todoListId)
        {
            TodoList? todoList = await this.todoListService.GetTodoListByIdAsync(todoListId);
            if (todoList is null)
            {
                return this.NotFound();
            }

            return this.Ok(todoList.ToTodoListApiModel());
        }

        [HttpGet("{todoListId:int}/tasks")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> GetTasks(int todoListId)
        {
            TodoList? todoList = await this.todoListService.GetTodoListByIdAsync(todoListId);
            if (todoList is null)
            {
                return this.NotFound();
            }

            List<Models.Task> tasks = await this.todoListService.GetTasksByTodoListIdAsync(todoListId);
            return this.Ok(tasks.Select(task => task.ToTaskApiModel()));
        }

        [HttpGet("{todoListId:int}/user/{userId}/tasks")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> GetTasksByUserId(int todoListId, string userId)
        {
            TodoList? todoList = await this.todoListService.GetTodoListByIdAsync(todoListId);
            if (todoList is null)
            {
                return this.NotFound();
            }

            List<Models.Task> tasks = await this.todoListService.GetTasksByTodoListIdAsync(todoListId);
            return this.Ok(tasks.Where(task => task.AssigneeId == userId).Select(task => task.ToTaskApiModel()));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> Create(TodoListApiModel newTodoList)
        {
            TodoList createdTodoList = await this.todoListService.CreateTodoListAsync(new TodoList(newTodoList));
            return this.CreatedAtAction(nameof(this.Create), createdTodoList.ToTodoListApiModel());
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> Update(TodoListApiModel todoListApiModel)
        {
            if (todoListApiModel is null)
            {
                return this.BadRequest();
            }

            TodoList? todoList = await this.todoListService.GetTodoListByIdAsync(todoListApiModel.Id);
            if (todoList is null)
            {
                return this.NotFound();
            }

            await this.todoListService.UpdateTodoListAsync(new TodoList(todoListApiModel));
            return this.NoContent();
        }

        [HttpDelete("{todoListId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
        [Produces(MediaTypeNames.Application.Json)]
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

        [HttpDelete("user/{userId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> DeleteTodoListsByUserId(string userId)
        {
            _ = await this.todoListService.GetTodoListsByUserIdAsync(userId);
            return this.NoContent();
        }
    }
}
