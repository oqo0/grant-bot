using Discord;
using Discord.Interactions;
using GrantBot.Data.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace GrantBot.AutocompleteHandlers;

public class SeasonAutocompleteHandler : AutocompleteHandler
{
    public override async Task<AutocompletionResult> GenerateSuggestionsAsync(
        IInteractionContext context,
        IAutocompleteInteraction autocompleteInteraction,
        IParameterInfo parameter,
        IServiceProvider services)
    {
        var seasonRepository = services.GetService<ISeasonRepository>();

        if (seasonRepository is null)
            return AutocompletionResult.FromError(new NullReferenceException());
        
        // max 25 suggestions at a time (Discord API limit)
        var lastSeasons = seasonRepository
            .GetSeasons(25)
            .Select(s => new AutocompleteResult(s.Name, s.Id));
        
        return AutocompletionResult.FromSuccess(lastSeasons);
    }
}