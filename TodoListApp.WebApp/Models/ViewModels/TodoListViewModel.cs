using System.ComponentModel.DataAnnotations;

namespace TodoListApp.WebApp.Models.ViewModels;

public class TodoListViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "To-do list must have a title")]
    [StringLength(30, MinimumLength = 2, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.")]
    public string Title { get; set; } = string.Empty;

    [StringLength(150, MinimumLength = 2, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.")]
    public string? Description { get; set; }

    public string? OwnerId { get; set; }

    public bool CurrentlyPicked { get; set; }

    public ListOfTasks? TasksList { get; set; }
}
