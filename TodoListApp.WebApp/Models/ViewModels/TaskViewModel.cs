using System.ComponentModel.DataAnnotations;
using TodoListApp.WebApp.Models.ViewModels.AuthenticationModels;

namespace TodoListApp.WebApp.Models.ViewModels;

public class TaskViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Task must have a title")]
    [StringLength(30, MinimumLength = 2, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.")]
    public string Title { get; set; } = string.Empty;

    [StringLength(100, MinimumLength = 2, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.")]
    public string? Description { get; set; }

    [Required]
    public DateTime CreationDate { get; set; }

    [Required(ErrorMessage = "Task must have a due date")]
    [DataType(DataType.Date)]
    public DateTime DueDate { get; set; }

    [Required]
    public StatusViewModel Status { get; set; } = new StatusViewModel();

    public ApplicationUser? Assignee { get; set; }

    [Required]
    public int TodoListId { get; set; }

    public ICollection<string>? Tags { get; init; }

    public ICollection<string>? Comments { get; init; }
}
