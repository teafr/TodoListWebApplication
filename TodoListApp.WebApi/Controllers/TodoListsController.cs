using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using TodoListApp.WebApi.Extensions;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApi.Services;

namespace TodoListApp.WebApi.Controllers;

[Authorize]
public class TodoListsController : BaseController
{
    private readonly ITodoListService todoListService;

    public TodoListsController(ITodoListService todoListService)
    {
        this.todoListService = todoListService;
    }

    /// <summary>
    /// Get all to-do lists.
    /// </summary>
    /// <returns>All to-do lists from DB.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> Get()
    {
        List<TodoList> todoLists = await this.todoListService.GetTodoListsAsync();
        Log.Debug("Successfully have got all to-do lists.");
        return this.Ok(todoLists.Select(todoList => todoList.ToTodoListApiModel()));
    }

    /// <summary>
    /// Get to-do lists by owner id.
    /// </summary>
    /// <param name="userId">Id of owner.</param>
    /// <returns>To-do lists of user.</returns>
    [HttpGet("user/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> GetByUserId(string userId)
    {
        List<TodoList> todoLists = await this.todoListService.GetTodoListsByUserIdAsync(userId);
        Log.Debug("Successfully have got to-do lists by user id {0}.", userId);
        return this.Ok(todoLists.Select(todoList => todoList.ToTodoListApiModel()));
    }

    /// <summary>
    /// Get to-do list by id.
    /// </summary>
    /// <param name="todoListId">Id of to-do list.</param>
    /// <returns>To-do list.</returns>
    [HttpGet("{todoListId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

    /// <summary>
    /// Create new to-do list.
    /// </summary>
    /// <param name="newTodoList">To-do list model.</param>
    /// <returns>Created to-do list.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> Create(TodoListApiModel newTodoList)
    {
        TodoList createdTodoList = await this.todoListService.CreateTodoListAsync(new TodoList(newTodoList));
        return this.CreatedAtAction(nameof(this.Create), createdTodoList.ToTodoListApiModel());
    }

    [HttpPost("{todoListId:int}/editors")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> AddEditor(int todoListId, [FromBody] string editorId)
    {
        TodoList? todoList = await this.todoListService.GetTodoListByIdAsync(todoListId);
        if (todoList is null)
        {
            return this.NotFound();
        }

        todoList.Editors!.Add(editorId);
        return await this.ExecuteWithValidation(() => this.todoListService.UpdateEditors(todoListId, todoList.Editors));
    }

    /// <summary>
    /// Update to-do list.
    /// </summary>
    /// <param name="todoListApiModel">To-do list model.</param>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> Update(TodoListApiModel todoListApiModel)
    {
        if (todoListApiModel is null)
        {
            Log.Warning("To-do list is null.");
            return this.BadRequest();
        }

        TodoList? todoList = await this.todoListService.GetTodoListByIdAsync(todoListApiModel.Id);
        if (todoList is null)
        {
            return this.NotFound();
        }

        return await this.ExecuteWithValidation(() => this.todoListService.UpdateTodoListAsync(new TodoList(todoListApiModel)));
    }

    /// <summary>
    /// Delete to-do list by id.
    /// </summary>
    /// <param name="todoListId">Id of the to-do list.</param>
    [HttpDelete("{todoListId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> Delete(int todoListId)
    {
        TodoList? todoList = await this.todoListService.GetTodoListByIdAsync(todoListId);
        if (todoList is null)
        {
            return this.NotFound();
        }

        return await this.ExecuteWithValidation(() => this.todoListService.DeleteTodoListByIdAsync(todoListId));
    }

    /// <summary>
    /// Delete all to-do lists by owner id.
    /// </summary>
    /// <param name="userId">Id of the owner.</param>
    [HttpDelete("user/{userId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> DeleteTodoListsByUserId(string userId)
    {
        return await this.ExecuteWithValidation(() => this.todoListService.DeleteTodoListsByUserIdAsync(userId));
    }

    [HttpDelete("{todoListId:int}/editors/{editorId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> RemoveEditor(int todoListId, string editorId)
    {
        TodoList? todoList = await this.todoListService.GetTodoListByIdAsync(todoListId);
        if (todoList is null)
        {
            return this.NotFound();
        }

        if (!todoList.Editors!.Remove(editorId))
        {
            Log.Warning("Editor with id {0} was not found in to-do list with id {1}.", editorId, todoListId);
            return this.BadRequest("Such editor doesn't exist in this to-do list.");
        }

        return await this.ExecuteWithValidation(() => this.todoListService.UpdateEditors(todoListId, todoList.Editors));
    }

    public override NotFoundResult NotFound()
    {
        Log.Warning("To-do list was not found.");
        return base.NotFound();
    }
}
