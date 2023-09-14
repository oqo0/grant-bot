using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GrantBot.Data.Models;

[Table("Awards")]
public class Award
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public ulong Id { get; set; }
    
    [Column, StringLength(255)]
    public string? Medal { get; set; }
    
    [Column]
    public DateTime ReceivedDateTime { get; set; }
    
    public ulong OwnerId { get; set; }
    
    [ForeignKey(nameof(OwnerId))]
    public virtual User User { get; set; }
    
    public ulong SeasonReceivedIn { get; set; }
    
    [ForeignKey(nameof(SeasonReceivedIn))]
    public virtual Season Season { get; set; }
}