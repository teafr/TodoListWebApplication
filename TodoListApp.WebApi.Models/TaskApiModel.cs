using System.ComponentModel.DataAnnotations;

namespace TodoListApp.WebApi.Models
{
    public class TaskApiModel
    {
        public int Id { get; set; }

        [Required, StringLength(30, MinimumLength = 2)]
        public string Title { get; set; } = string.Empty;

        [StringLength(100, MinimumLength = 2)]
        public string? Description { get; set; }

        [Required, StringLength(40, MinimumLength = 1)]
        public string? AssigneeId { get; set; } = string.Empty;

        [Required]
        public DateTime CreationDate { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        public ICollection<string>? Tags { get; init; }

        public ICollection<string>? Comments { get; init; }

        public int TodoListId { get; set; }

        [Required]
        public StatusApiModel Status { get; set; } = new StatusApiModel();
    }
}
