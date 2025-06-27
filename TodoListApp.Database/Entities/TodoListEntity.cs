using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoListApp.Database.Entities;

[Table("todo_lists")]
public class TodoListEntity : IDatabaseEntity
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

    [Column("owner_id")]
    [Required, StringLength(40, MinimumLength = 1)]
    public string OwnerId { get; set; } = string.Empty;

    [Column("editors")]
    public string? Editors { get; set; }

    public ICollection<TaskEntity> Tasks { get; init; } = new List<TaskEntity>();
}
