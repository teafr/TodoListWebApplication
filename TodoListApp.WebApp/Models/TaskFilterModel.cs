namespace TodoListApp.WebApp.Models;

public class TaskFilterModel
{
    public string? SortBy { get; set; }

    public string? Tag { get; set; }

    public int StatusId { get; set; }

    public int CurrentPage { get; set; } = 1;

    public bool ShowCompletedTasks { get; set; }
}
