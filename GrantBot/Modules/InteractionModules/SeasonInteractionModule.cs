using Discord;
using Discord.Interactions;
using GrantBot.Data.Models;
using GrantBot.Data.Repositories;
using GrantBot.Extensions;
using GrantBot.Models.Modals;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SmartFormat;
using ContextType = Discord.Interactions.ContextType;

namespace GrantBot.Modules.InteractionModules;

[RequireUserPermission(GuildPermission.Administrator)]
[RequireContext(ContextType.Guild)]
[EnabledInDm(false)]
public class SeasonInteractionModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly ISeasonRepository _seasonRepository;
    private readonly ILogger<SeasonInteractionModule> _logger;
    private readonly IConfiguration _configuration;

    public SeasonInteractionModule(
        ISeasonRepository seasonRepository,
        ILogger<SeasonInteractionModule> logger,
        IConfiguration configuration)
    {
        _seasonRepository = seasonRepository;
        _logger = logger;
        _configuration = configuration;
    }
    
    [SlashCommand("season-new", "Starts a new season.")]
    public async Task ShowNewSeasonModal()
    {
        await Context.Interaction.RespondWithModalAsync<StartNewSeasonModal>(
            "gb.new_season",
            modifyModal: modal =>
            {
                modal.Title = _configuration["lang:season:modal-title"];
                modal.UpdateTextInput(
                    "gb.new_season.start", 
                    input => input.Label = _configuration["lang:season:modal-input"]);
            });
    }
    
    [ModalInteraction("gb.new_season")]
    public async Task CreateNewSeason(StartNewSeasonModal modal)
    {
        if (string.IsNullOrEmpty(modal.SeasonName))
            return;

        var newSeason = new Season()
        {
            Name = modal.SeasonName,
            StartDateTime = DateTime.UtcNow
        };

        var newSeasonId = _seasonRepository.Create(newSeason);

        await RespondAsync(
            Smart.Format(_configuration["lang:season:season-created"],
                new { newSeason.Name, Id = newSeasonId }),
            ephemeral: true);
        
        _logger.LogInformation(
            "User {UserName} ({UserId}) created a new season with id {NewSeasonId}.",
            Context.User.Username, Context.User.Id, newSeasonId);
    }
}