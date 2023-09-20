using Discord;
using Discord.Interactions;
using GrantBot.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GrantBot.AutocompleteHandlers;

public class AwardAutocompleteHandler : AutocompleteHandler
{
    public override async Task<AutocompletionResult> GenerateSuggestionsAsync(
        IInteractionContext context,
        IAutocompleteInteraction autocompleteInteraction,
        IParameterInfo parameter,
        IServiceProvider services)
    {
        var configuration = services.GetService<IConfiguration>();
        
        if (configuration is null)
            return AutocompletionResult.FromError(new NullReferenceException());
        
        var registeredAwards = configuration.GetSection("awards").Get<List<AwardConfig>>();

        var results = registeredAwards
            .Select(award => new AutocompleteResult(award.Name, award.Id))
            .ToList();

        // max 25 suggestions at a time (Discord API limit)
        return AutocompletionResult.FromSuccess(results.Take(25));
    }
}