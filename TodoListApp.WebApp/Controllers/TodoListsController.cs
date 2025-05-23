using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApp.Extensions;
using TodoListApp.WebApp.Models;
using TodoListApp.WebApp.Models.ViewModels;
using TodoListApp.WebApp.Services;

namespace TodoListApp.WebApp.Controllers;

[Authorize]
public class TodoListsController : Controller
{
    private readonly ITodoListWebApiService apiService;
    private readonly UserManager<IdentityUser> userManager;

    public TodoListsController(ITodoListWebApiService apiService, UserManager<IdentityUser> userManager)
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

            return this.View(todoList.ToTodoListViewModel(currentPage));
        }

        return this.View("Error", new ErrorViewModel { RequestId = "Invalid Model State" });
    }

    public async Task<IActionResult> Create()
    {
        return this.View(new TodoListViewModel() { OwnerId = (await this.userManager.GetUserAsync(this.User)).Id });
    }

    [HttpPost]
    public async Task<IActionResult> Create(TodoListViewModel todoListViewModel)
    {
        if (this.ModelState.IsValid)
        {
            await this.apiService.CreateTodoListAsync(new TodoListModel(todoListViewModel));
            return this.RedirectToAction("Index");
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

            return this.View(todoList.ToTodoListViewModel() ?? new TodoListViewModel());
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
}
