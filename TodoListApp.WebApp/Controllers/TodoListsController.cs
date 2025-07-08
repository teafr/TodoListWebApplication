using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using TodoListApp.WebApp.Extensions;
using TodoListApp.WebApp.Helpers;
using TodoListApp.WebApp.Models;
using TodoListApp.WebApp.Models.ViewModels;
using TodoListApp.WebApp.Models.ViewModels.AuthenticationModels;
using TodoListApp.WebApp.Services;

namespace TodoListApp.WebApp.Controllers;

[Authorize]
public class TodoListsController : Controller
{
    private readonly ITodoListWebApiService apiService;
    private readonly UserManager<ApplicationUser> userManager;

    public TodoListsController(ITodoListWebApiService apiService, UserManager<ApplicationUser> userManager)
    {
        this.apiService = apiService;
        this.userManager = userManager;
    }

    public IActionResult Index()
    {
        return this.View();
    }

    public async Task<IActionResult> GetTasks(int todoListId, int currentPage = 1)
    {
        if (this.ModelState.IsValid)
        {
            this.ViewBag.CurrentTodoListId = todoListId;
            TodoListModel? todoList = await this.apiService.GetTodoListByIdAsync(todoListId);
            if (todoList is null)
            {
                return this.NotFound();
            }

            return this.View(todoList.ToTodoListViewModel(this.userManager, currentPage));
        }

        return this.View("Error", new ErrorViewModel { RequestId = "Invalid Model State" });
    }

    public IActionResult Create()
    {
        return this.View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(TodoListViewModel todoListViewModel)
    {
        if (this.ModelState.IsValid && todoListViewModel is not null)
        {
            ApplicationUser? currentUser = await this.userManager.GetUserAsync(this.User);
            todoListViewModel.Owner = currentUser;
            todoListViewModel.Editors!.Add(currentUser);

            var result = await this.apiService.CreateTodoListAsync(new TodoListModel(todoListViewModel));

            if (result.StatusCode == HttpStatusCode.Created)
            {
                Log.Information("To-do list created successfully.");
                return this.RedirectToAction("Index");
            }
            else if (result.StatusCode == HttpStatusCode.BadRequest)
            {
                Log.Warning("Failed to create a new to-do list due to bad request.");
                this.ModelState.AddModelError(string.Empty, "Invalid to-do list");
            }

            Log.Error("Failed to create a new to-do list. Status code: {StatusCode}", result.StatusCode);
            return this.View("Error", new ErrorViewModel { RequestId = "We couldn't create a new to-do list. Please try again later." });
        }

        return this.View();
    }

    public async Task<IActionResult> AddEditor(int todoListId, string editorId)
    {
        if (this.ModelState.IsValid)
        {
            ApplicationUser? editor = await this.userManager.FindByIdAsync(editorId);
            if (editor is null)
            {
                Log.Warning("Attempted to add editor with ID {EditorId} to list {TodoListId}, but user does not exist", editorId, todoListId);
                return this.View("Error", new ErrorViewModel { RequestId = "Such user doesn't exist" });
            }

            List<int> lists = JsonSerializer.Deserialize<List<int>>(string.IsNullOrEmpty(editor.HasAccsses) ? "[]" : editor.HasAccsses) ?? new List<int>();
            lists.Add(todoListId);
            editor.HasAccsses = JsonSerializer.Serialize(lists);

            if ((await this.userManager.UpdateAsync(editor)).Succeeded)
            {
                var result = await this.apiService.AddEditorAsync(todoListId, editorId);

                if (result.StatusCode == HttpStatusCode.NoContent)
                {
                    Log.Information("Editor {EditorId} added to list {TodoListId} successfully", editorId, todoListId);
                    return this.RedirectToAction("GetTasks", new { todoListId });
                }
                else if (result.StatusCode == HttpStatusCode.NotFound)
                {
                    return this.NotFound();
                }

                _ = lists.Remove(todoListId);
                editor.HasAccsses = JsonSerializer.Serialize(lists);
                _ = await this.userManager.UpdateAsync(editor);

                Log.Error("Failed to add editor {EditorId} to list {TodoListId}. Status code: {StatusCode}", editorId, todoListId, result.StatusCode);
                return this.View("Error", new ErrorViewModel { RequestId = "We couldn't add the editor to this list. Please try again later." });
            }
        }

        return this.View("Error", new ErrorViewModel { RequestId = "Invalid Model State" });
    }

    public async Task<IActionResult> Edit(int todoListId)
    {
        if (this.ModelState.IsValid)
        {
            var todoList = await this.apiService.GetTodoListByIdAsync(todoListId);
            if (todoList is null)
            {
                return this.NotFound();
            }

            return this.View(todoList.ToTodoListViewModel(this.userManager) ?? new TodoListViewModel());
        }

        return this.View("Error", new ErrorViewModel { RequestId = "Invalid Model State" });
    }

    [HttpPost]
    public async Task<IActionResult> Edit(TodoListViewModel todoListViewModel)
    {
        ExceptionHelper.CheckObjectForNull(todoListViewModel);

        if (this.ModelState.IsValid)
        {
            var result = await this.apiService.UpdateTodoListAsync(new TodoListModel(todoListViewModel));
            if (result.StatusCode == HttpStatusCode.NoContent)
            {
                Log.Information("To-do list with ID {TodoListId} updated successfully", todoListViewModel.Id);
                return this.RedirectToAction("GetTasks", new { todoListId = todoListViewModel.Id });
            }
            else if (result.StatusCode == HttpStatusCode.NotFound)
            {
                return this.NotFound();
            }
            else if (result.StatusCode == HttpStatusCode.BadRequest)
            {
                Log.Warning("Failed to update the to-do list due to bad request. Model state: {ModelState}", this.ModelState);
                return this.View("Error", new ErrorViewModel { RequestId = "Invalid input" });
            }

            Log.Error("Failed to update the to-do list. Status code: {StatusCode}", result.StatusCode);
            return this.View("Error", new ErrorViewModel { RequestId = "We couldn't update the to-do list. Please try again later." });
        }

        return this.View(todoListViewModel);
    }

    public async Task<IActionResult> Delete(int todoListId)
    {
        if (this.ModelState.IsValid)
        {
            var result = await this.apiService.DeleteTodoListAsync(todoListId);
            if (result.StatusCode == HttpStatusCode.NoContent)
            {
                Log.Information("To-do list with ID {TodoListId} deleted successfully", todoListId);
                return this.RedirectToAction("Index");
            }
            else if (result.StatusCode == HttpStatusCode.NotFound)
            {
                return this.NotFound();
            }

            Log.Error("Failed to delete the to-do list with ID {TodoListId}. Status code: {StatusCode}", todoListId, result.StatusCode);
            return this.View("Error", new ErrorViewModel { RequestId = "We couldn't delete the to-do list. Please try again later." });
        }

        return this.View("Error", new ErrorViewModel { RequestId = "Invalid Model State" });
    }

    public async Task<IActionResult> RemoveEditor(int todoListId, string editorId)
    {
        if (this.ModelState.IsValid)
        {
            ApplicationUser? editor = await this.userManager.FindByIdAsync(editorId);
            if (editor is null)
            {
                Log.Warning("Attempted to remove editor with ID {EditorId} from list {TodoListId}, but such user does not exist", editorId, todoListId);
                return this.View("Error", new ErrorViewModel { RequestId = "Such user doesn't exist" });
            }

            List<int> lists = JsonSerializer.Deserialize<List<int>>(string.IsNullOrEmpty(editor.HasAccsses) ? "[]" : editor.HasAccsses) ?? new List<int>();
            if (!lists.Contains(todoListId))
            {
                Log.Warning("Editor {EditorId} does not have access to list {TodoListId}, so removal was not successful", editorId, todoListId);
                return this.View("Error", new ErrorViewModel { RequestId = "This user doesn't have access to this list" });
            }

            if (lists.Remove(todoListId))
            {
                editor.HasAccsses = JsonSerializer.Serialize(lists);
                if (!(await this.userManager.UpdateAsync(editor)).Succeeded)
                {
                    return this.View("Error", new ErrorViewModel { RequestId = "Failed to add editor to list" });
                }

                var result = await this.apiService.RemoveEditorAsync(todoListId, editorId);
                if (result.StatusCode == HttpStatusCode.NoContent)
                {
                    return this.RedirectToAction("GetTasks", new { todoListId });
                }
                else if (result.StatusCode == HttpStatusCode.NotFound)
                {
                    return this.NotFound();
                }
                else if (result.StatusCode == HttpStatusCode.BadRequest)
                {
                    Log.Warning("Failed to remove editor {EditorId} from list {TodoListId} due to bad request", editorId, todoListId);
                    return this.View("Error", new ErrorViewModel { RequestId = "Invalid input" });
                }
            }
            else
            {
                return this.View("Error", new ErrorViewModel { RequestId = "This user doesn't have access to this list, so removing wasn't successful" });
            }

            lists.Add(todoListId);
            editor.HasAccsses = JsonSerializer.Serialize(lists);
            _ = await this.userManager.UpdateAsync(editor);
            return this.View("Error", new ErrorViewModel { RequestId = "Failed to remove editor from list" });
        }

        return this.View("Error", new ErrorViewModel { RequestId = "Invalid Model State" });
    }

    public override NotFoundResult NotFound()
    {
        Log.Warning("To-do list was not found");
        return base.NotFound();
    }
}
