using System.ComponentModel.DataAnnotations;

namespace TodoListApp.WebApi.Models
{
    public class TodoListApiModel
    {
        public int Id { get; set; }

        [Required, StringLength(30, MinimumLength = 2)]
        public string Title { get; set; } = string.Empty;

        [StringLength(100, MinimumLength = 2)]
        public string? Description { get; set; }

        [Required, StringLength(40, MinimumLength = 1)]
        public string OwnerId { get; set; } = string.Empty;

        public ICollection<TaskApiModel>? Tasks { get; init; } = new List<TaskApiModel>();
    }
}
