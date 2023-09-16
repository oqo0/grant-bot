using System.ComponentModel.DataAnnotations;
using Discord.Interactions;

namespace GrantBot.Models.Modals;

public class StartNewSeasonModal : IModal
{
    [Required]
    public string? Title { get; set; }
    
    [ModalTextInput("gb.new_season.start")]
    [Required]
    public string? SeasonName { get; set; }
}