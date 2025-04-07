using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.VisualBasic;

namespace TodoListApp.WebApi.Entities;

[Table("todo_lists")]
public class TodoListEntity : IDatabaseEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("title")]
    [Required]
    public string Title { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    [Column("owner_id")]
    public string OwnerId { get; set; }

    public Collection<TaskEntity> Tasks { get; set; }
}
