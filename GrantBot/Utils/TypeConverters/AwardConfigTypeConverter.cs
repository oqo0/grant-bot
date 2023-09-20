using Discord;
using Discord.Interactions;
using GrantBot.Extensions;
using GrantBot.Models;
using Microsoft.Extensions.DependencyInjection;

namespace GrantBot.Utils.TypeConverters;

public class AwardConfigTypeConverter : TypeConverter
{
    public override bool CanConvertTo(Type type)
    {
        return type.IsAssignableFrom(typeof(AwardConfig));
    }

    public override Task<TypeConverterResult> ReadAsync(
        IInteractionContext context,
        IApplicationCommandInteractionDataOption option,
        IServiceProvider services)
    {
        var registeredAwards = services.GetService<IList<AwardConfig>>();

        if (registeredAwards is null)
            return this.Error(
                InteractionCommandError.Exception,
                $"Service {nameof(IList<AwardConfig>)} was not found.");
        
        var award = registeredAwards.FirstOrDefault(a => a.Id == option.Value.ToString());

        if (award is null)
            return this.Error(
                InteractionCommandError.ConvertFailed,
                $"{nameof(AwardConfig)} was not found.");
        
        return Task.FromResult(TypeConverterResult.FromSuccess(award));
    }

    public override ApplicationCommandOptionType GetDiscordType()
        => ApplicationCommandOptionType.String;
}