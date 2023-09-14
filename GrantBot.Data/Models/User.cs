using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GrantBot.Data.Models;

[Table("Users")]
public class User
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public ulong Id { get; set; }
    
    [Column, MaxLength(255)]
    public string? Rank { get; set; }
    
    [InverseProperty(nameof(Award.User))]
    public virtual ICollection<Award> Awards { get; set; } = new List<Award>();
}