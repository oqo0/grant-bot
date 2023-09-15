using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GrantBot.Data.Models;

[Table("Seasons")]
public class Season
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public ulong Id { get; set; }
    
    [Column, MaxLength(255)]
    public string? Name { get; set; }
    
    [Column]
    public DateTime StartDateTime { get; set; }
    
    [InverseProperty(nameof(Award.Season))]
    public virtual ICollection<Award> Awards { get; set; } = new List<Award>();
}