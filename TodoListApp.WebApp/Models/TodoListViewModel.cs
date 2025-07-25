using System.ComponentModel.DataAnnotations;
using TodoListApp.WebApp.Models.AuthenticationModels;

namespace TodoListApp.WebApp.Models;

public class TodoListViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "To-do list must have a title")]
    [StringLength(30, MinimumLength = 2, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.")]
    public string Title { get; set; } = string.Empty;

    [StringLength(100, MinimumLength = 2, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.")]
    public string? Description { get; set; }

    public ApplicationUser? Owner { get; set; }

    public ICollection<ApplicationUser>? Editors { get; init; } = new List<ApplicationUser>();

    public bool CurrentlyPicked { get; set; }

    public TodoListTasks? TasksList { get; set; }
}
