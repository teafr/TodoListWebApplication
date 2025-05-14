using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoListApp.Database.Entities;

[Table("tasks")]
public class TaskEntity : IDatabaseEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("title")]
    [Required, StringLength(30, MinimumLength = 2)]
    public string Title { get; set; } = string.Empty;

    [Column("description")]
    [StringLength(100, MinimumLength = 2)]
    public string? Description { get; set; }

    [Column("creation_date")]
    public DateTime CreationDate { get; set; }

    [Column("due_date")]
    public DateTime DueDate { get; set; }

    [Column("status_id")]
    [ForeignKey("Status")]
    [Required, Range(1, 3)]
    public int StatusId { get; set; }

    [Column("assignee_id")]
    [Required, StringLength(40, MinimumLength = 1)]
    public string AssigneeId { get; set; } = string.Empty;

    [Column("todo_list_id")]
    [ForeignKey("TodoList")]
    public int TodoListId { get; set; }

    [Column("tag")]
    public string? Tags { get; set; }

    [Column("comment")]
    public string? Comments { get; set; }

    [Required]
    public StatusEntity Status { get; set; } = new StatusEntity();

    public TodoListEntity? TodoList { get; set; }
}
