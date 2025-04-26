using TodoListApp.WebApp.Extensions;
using TodoListApp.WebApp.Helpers;

namespace TodoListApp.WebApp.Models.ViewModels;

public class ListOfTodoListTasks
{
    public ListOfTodoListTasks(TodoListModel todoListModel)
    {
        ExceptionHelper.CheckViewModel(todoListModel);

        if (todoListModel.Tasks is not null)
        {
            foreach (var task in todoListModel.Tasks)
            {
                this.Tasks.Add(task.ToTaskViewModel());
            }
        }

        this.TodoListId = todoListModel.Id;
        this.Description = todoListModel.Description;
        this.PaginationInfo.CountOfItems = this.Tasks.Count;
        this.PaginationInfo.PageSize = 5;
        this.PaginationInfo.CurrentPage = 1;
    }

    public ICollection<TaskViewModel> Tasks { get; init; } = new List<TaskViewModel>();

    public PaginationInfo PaginationInfo { get; set; } = new PaginationInfo();

    public int TodoListId { get; set; }

    public string? Description { get; set; }
}
