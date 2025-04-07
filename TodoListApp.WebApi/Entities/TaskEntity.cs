using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoListApp.WebApi.Entities;

[Table("tasks")]
public class TaskEntity : IDatabaseEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("title")]
    public string Title { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    [Column("creation_date")]
    public DateTime CreationDate { get; set; }

    [Column("due_date")]
    public DateTime DueDate { get; set; }

    [Column("assignee_id")]
    public string AssigneeId { get; set; }

    [Column("status_id")]
    [ForeignKey("Status")]
    public int StatusId { get; set; }

    [Column("todo_list_id")]
    [ForeignKey("TodoList")]
    public int TodoListId { get; set; }

    public Collection<string>? Tags { get; set; }

    public Collection<string>? Comments { get; set; }

    [Required]
    public StatusEntity Status { get; set; }

    [Required]
    public TodoListEntity TodoList { get; set; }
}
