using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TodoListApp.Database.Entities;

[Table("statuses")]
public class StatusEntity : IDatabaseEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    [Required, StringLength(20)]
    public string Name { get; set; } = string.Empty;

    [JsonIgnore]
    public ICollection<TaskEntity>? Tasks { get; init; }
}
