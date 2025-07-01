using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApp.Models;
using TodoListApp.WebApp.Models.ViewModels;
using TodoListApp.WebApp.Models.ViewModels.AuthenticationModels;
using TodoListApp.WebApp.Services;

namespace TodoListApp.WebApp.Components;

[Authorize]
public class TodoListMenuViewComponent : ViewComponent
{
    private readonly ITodoListWebApiService todoListWebApiService;
    private readonly UserManager<ApplicationUser> userManager;

    public TodoListMenuViewComponent(ITodoListWebApiService todoListWebApiService, UserManager<ApplicationUser> userManager)
    {
        this.todoListWebApiService = todoListWebApiService;
        this.userManager = userManager;
    }

    public async Task<IViewComponentResult> InvokeAsync(int? selectedTodoListId)
    {
        var userId = this.userManager.GetUserId(this.UserClaimsPrincipal);
        var todoLists = await this.todoListWebApiService.GetTodoListsByUserIdAsync(userId);

        ListOfTodoLists listOfTodoLists = new ListOfTodoLists(todoLists ?? new List<TodoListModel>(), this.userManager);
        listOfTodoLists.TodoLists.ToList().ForEach(todoList => todoList.CurrentlyPicked = selectedTodoListId.HasValue && todoList.Id == selectedTodoListId);

        return this.View(listOfTodoLists);
    }
}
