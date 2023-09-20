using Discord.Interactions;

namespace GrantBot.Extensions;

public static class TypeConverterExtension
{
    public static Task<TypeConverterResult> Error(
        this TypeConverter typeConverter,
        InteractionCommandError commandError,
        string errorReason)
    {
        return Task.FromResult(TypeConverterResult.FromError(commandError, errorReason));
    }
}