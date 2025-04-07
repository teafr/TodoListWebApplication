using System.ComponentModel.DataAnnotations;

namespace TodoListApp.WebApi.Models
{
    public class StatusApiModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
