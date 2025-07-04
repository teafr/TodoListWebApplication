using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApp.Extensions;
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

            await this.apiService.CreateTodoListAsync(new TodoListModel(todoListViewModel));

            return this.RedirectToAction("Index");
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
                return this.View("Error", new ErrorViewModel { RequestId = "Such user doesn't exist" });
            }

            List<int> lists = JsonSerializer.Deserialize<List<int>>(string.IsNullOrEmpty(editor.HasAccsses) ? "[]" : editor.HasAccsses) ?? new List<int>();
            lists.Add(todoListId);
            editor.HasAccsses = JsonSerializer.Serialize(lists);

            if (!(await this.userManager.UpdateAsync(editor)).Succeeded)
            {
                return this.View("Error", new ErrorViewModel { RequestId = "Failed to add editor to list" });
            }

            var todoList = await this.apiService.GetTodoListByIdAsync(todoListId);
            if (todoList is null)
            {
                return this.NotFound();
            }

            await this.apiService.AddEditorAsync(todoListId, editorId);
            return this.RedirectToAction("GetTasks", new { todoListId });
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
        if (this.ModelState.IsValid)
        {
            await this.apiService.UpdateTodoListAsync(new TodoListModel(todoListViewModel));
            return this.RedirectToAction("GetTasks", new { todoListId = todoListViewModel.Id });
        }

        return this.View("Error", new ErrorViewModel { RequestId = "Invalid Model State" });
    }

    public async Task<IActionResult> Delete(int todoListId)
    {
        if (this.ModelState.IsValid)
        {
            var todoList = await this.apiService.GetTodoListByIdAsync(todoListId);
            if (todoList is null)
            {
                return this.NotFound();
            }

            await this.apiService.DeleteTodoListAsync(todoListId);
            return this.RedirectToAction("Index");
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
                return this.View("Error", new ErrorViewModel { RequestId = "Such user doesn't exist" });
            }

            var todoList = await this.apiService.GetTodoListByIdAsync(todoListId);
            if (todoList is null)
            {
                return this.NotFound();
            }

            List<int> lists = JsonSerializer.Deserialize<List<int>>(string.IsNullOrEmpty(editor.HasAccsses) ? "[]" : editor.HasAccsses) ?? new List<int>();
            if (lists.Remove(todoListId))
            {
                editor.HasAccsses = JsonSerializer.Serialize(lists);
                if (!(await this.userManager.UpdateAsync(editor)).Succeeded)
                {
                    return this.View("Error", new ErrorViewModel { RequestId = "Failed to add editor to list" });
                }
            }
            else
            {
                return this.View("Error", new ErrorViewModel { RequestId = "This user doesn't have access to this list, so removing wasn't successful" });
            }

            await this.apiService.RemoveEditorAsync(todoListId, editorId);
            return this.RedirectToAction("GetTasks", new { todoListId });
        }

        return this.View("Error", new ErrorViewModel { RequestId = "Invalid Model State" });
    }
}
