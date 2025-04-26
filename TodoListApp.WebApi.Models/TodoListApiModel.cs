using System.ComponentModel.DataAnnotations;

namespace TodoListApp.WebApi.Models
{
    public class TodoListApiModel
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public string OwnerId { get; set; } = string.Empty;

        public ICollection<TaskApiModel>? Tasks { get; init; } = new List<TaskApiModel>();
    }
}
