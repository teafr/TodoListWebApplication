using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using TodoListApp.WebApi.Models;

namespace TodoListApp.WebApp.Models;

public class TodoList
{
    public int Id { get; set; }

    [Required]
    public string Title { get; set; }

    public string? Description { get; set; }

    [Required]
    public IdentityUser Owner { get; set; }

    public Collection<TaskApiModel>? Tasks { get; set; }
}
