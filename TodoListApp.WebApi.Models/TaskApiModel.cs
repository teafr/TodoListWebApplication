using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace TodoListApp.WebApi.Models
{
    public class TaskApiModel
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string? Description { get; set; }

        public string AssigneeId { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime DueDate { get; set; }

        public Collection<string>? Tags { get; set; }

        public Collection<string>? Comments { get; set; }

        [Required]
        public StatusApiModel Status { get; set; }
    }
}
