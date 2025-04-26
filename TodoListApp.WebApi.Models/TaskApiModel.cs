using System.ComponentModel.DataAnnotations;

namespace TodoListApp.WebApi.Models
{
    public class TaskApiModel
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public string AssigneeId { get; set; } = string.Empty;

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
