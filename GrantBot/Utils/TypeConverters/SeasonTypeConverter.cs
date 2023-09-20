using Discord;
using Discord.Interactions;
using GrantBot.Data.Models;
using GrantBot.Data.Repositories;
using GrantBot.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace GrantBot.Utils.TypeConverters;

public class SeasonTypeConverter : TypeConverter
{
    public override bool CanConvertTo(Type type)
    {
        return type.IsAssignableFrom(typeof(Season));
    }

    public override Task<TypeConverterResult> ReadAsync(
        IInteractionContext context,
        IApplicationCommandInteractionDataOption option,
        IServiceProvider services)
    {
        var seasonRepository = services.GetService<ISeasonRepository>();

        if (seasonRepository is null)
            return this.Error(
                InteractionCommandError.ConvertFailed,
                $"Service {nameof(ISeasonRepository)} was not found.");

        if (!long.TryParse(option.Value.ToString(), out long seasonId))
            return this.Error(
                InteractionCommandError.ConvertFailed,
                $"Service {nameof(ISeasonRepository)} was not found.");
        
        var season = seasonRepository.GetById(seasonId);

        return Task.FromResult(TypeConverterResult.FromSuccess(season));
    }

    public override ApplicationCommandOptionType GetDiscordType()
        => ApplicationCommandOptionType.Integer;
}