using Microsoft.AspNetCore.Identity;
using TodoListApp.WebApp.Extensions;
using TodoListApp.WebApp.Models.ViewModels.AuthenticationModels;

namespace TodoListApp.WebApp.Models.ViewModels;

public class ListOfTodoLists
{
    public ListOfTodoLists(IEnumerable<TodoListModel> todoLists, UserManager<ApplicationUser> userManager)
    {
        this.TodoLists = todoLists.Select(todoList => todoList.ToTodoListViewModel(userManager)).ToList();

        this.PaginationInfo = new PaginationInfo()
        {
            CountOfItems = this.TodoLists.Count,
            PageSize = 5,
            CurrentPage = 1,
        };
    }

    public ICollection<TodoListViewModel> TodoLists { get; init; }

    public PaginationInfo PaginationInfo { get; set; } = new PaginationInfo();
}
