using Discord.Addons.Hosting;
using Discord.Addons.Hosting.Util;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GrantBot.Modules.ClientServices;

public class PlayGameServiceModule : DiscordClientService
{
    private readonly IConfiguration _configuration;
    
    public PlayGameServiceModule(
        DiscordSocketClient client,
        ILogger<PlayGameServiceModule> logger,
        IConfiguration configuration
    ) : base(client, logger)
    {
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Client.WaitForReadyAsync(stoppingToken);
        
        bool isPresenceEnabled = _configuration.GetValue<bool>("presence:enabled");
        if (isPresenceEnabled)
        {
            string gameName = _configuration["presence:game-name"];
            await Client.SetGameAsync(gameName);
        }
    }
}