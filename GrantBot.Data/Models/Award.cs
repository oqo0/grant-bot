using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GrantBot.Data.Models;

[Table("Awards")]
public class Award
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    
    [Column, StringLength(255)]
    public string? UniqueId { get; set; }
    
    [Column, StringLength(255)]
    public string? Medal { get; set; }
    
    [Column]
    public uint Weight { get; set; }
    
    [Column]
    public DateTime ReceivedDateTime { get; set; }
    
    public long OwnerId { get; set; }
    
    [ForeignKey(nameof(OwnerId))]
    public virtual User User { get; set; }
    
    public long SeasonReceivedIn { get; set; }
    
    [ForeignKey(nameof(SeasonReceivedIn))]
    public virtual Season Season { get; set; }
}