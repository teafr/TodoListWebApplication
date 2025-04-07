using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Identity;
using TodoListApp.WebApi.Models;

namespace TodoListApp.WebApp.Models;

public class Task
{
    public int Id { get; set; }

    public string Title { get; set; }

    public string? Description { get; set; }

    public IdentityUser Assignee { get; set; }

    public DateTime CreationDate { get; set; }

    public DateTime DueDate { get; set; }

    public Collection<string>? Tags { get; set; }

    public Collection<string>? Comments { get; set; }

    public StatusApiModel Status { get; set; }
}
