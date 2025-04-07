using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace TodoListApp.WebApi.Models
{
    public class TodoListApiModel
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string? Description { get; set; }

        [Required]
        public string OwnerId { get; set; }

        public Collection<TaskApiModel>? Tasks { get; set; }
    }
}
