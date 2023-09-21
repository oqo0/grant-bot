using System.ComponentModel;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using GrantBot.AutocompleteHandlers;
using GrantBot.Data.Models;
using GrantBot.Data.Repositories;
using GrantBot.Services.Painters;
using GrantBot.Utils.TypeConverters;
using SixLabors.ImageSharp.Formats.Png;

namespace GrantBot.Modules.InteractionModules;

[RequireContext(ContextType.Guild)]
[EnabledInDm(false)]
public class ProfileModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IProfileInfoPainter _profileInfoPainter;
    private readonly IUserRepository _userRepository;

    public ProfileModule(
        IProfileInfoPainter profileInfoPainter,
        IUserRepository userRepository)
    {
        _profileInfoPainter = profileInfoPainter;
        _userRepository = userRepository;
    }

    [SlashCommand("profile", "Show user profile information.")]
    public async Task Profile(
        SocketGuildUser guildUser,
        [Autocomplete(typeof(SeasonAutocompleteHandler)),
         TypeConverter(typeof(SeasonTypeConverter))] Season season)
    {
        var user = _userRepository.GetByDiscordId(guildUser.Id);
        
        if (user is null)
        {
            await RespondAsync("not found");
            return;
        }
                
        using var stream = new MemoryStream();
        var awardReceivedImage = _profileInfoPainter.Draw(user, season.Id);
        await awardReceivedImage.SaveAsync(stream, new PngEncoder());
    
        await RespondWithFileAsync(
            new FileAttachment(stream, "userProfile.png"),
            "This is an image");
    }
}