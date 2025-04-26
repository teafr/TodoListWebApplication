using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

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
    public string? TagsSerialized { get; set; }

    [NotMapped]
    public ICollection<string>? Tags
    {
        get => string.IsNullOrEmpty(this.TagsSerialized) ? new List<string>() : JsonSerializer.Deserialize<List<string>>(this.TagsSerialized);
        set => this.TagsSerialized = JsonSerializer.Serialize(value);
    }

    [Column("comment")]
    public string? CommentsSerialized { get; set; }

    [NotMapped]
    public ICollection<string>? Comments
    {
        get => string.IsNullOrEmpty(this.CommentsSerialized) ? new List<string>() : JsonSerializer.Deserialize<List<string>>(this.CommentsSerialized);
        set => this.CommentsSerialized = JsonSerializer.Serialize(value);
    }

    [Required]
    public StatusEntity Status { get; set; } = new StatusEntity();

    public TodoListEntity? TodoList { get; set; }
}
