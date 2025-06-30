using TodoListApp.WebApp.Extensions;

namespace TodoListApp.WebApp.Models.ViewModels;

public class TodoListTasks : PaginationViewModel
{
    public TodoListTasks(TodoListModel todoList, string userId, int currentPage = 1)
        : base(todoList?.Tasks?.Count ?? 0, currentPage, 4)
    {
        this.Tasks = todoList?.Tasks?.Select(task => task.ToTaskViewModel()).ToList() ?? new List<TaskViewModel>();
        this.CanEdit = todoList?.OwnerId == userId || (todoList?.Editors?.Contains(userId) ?? false);
    }

    public ICollection<TaskViewModel> Tasks { get; init; }

    public bool CanEdit { get; set; }
}
