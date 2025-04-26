using TodoListApp.WebApp.Extensions;

namespace TodoListApp.WebApp.Models.ViewModels;

public class ListOfTodoLists
{
    public ListOfTodoLists(IEnumerable<TodoListModel> todoLists)
    {
        this.TodoLists = todoLists.Select(todoList => todoList.ToTodoListViewModel()).ToList();

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
