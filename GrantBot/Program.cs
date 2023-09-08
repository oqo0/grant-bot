using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using GrantBot.Modules;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GrantBot;

public class Program
{
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _services;

    private readonly DiscordSocketConfig _socketConfig = new()
    {
        GatewayIntents = GatewayIntents.AllUnprivileged,
        AlwaysDownloadUsers = true,
    };

    private Program()
    {
        _configuration = new ConfigurationBuilder()
            .AddYamlFile("config.yml")
            .Build();

        _services = new ServiceCollection()
            .AddSingleton(_configuration)
            .AddSingleton(_socketConfig)
            .AddSingleton<DiscordSocketClient>()
            .AddSingleton(sp => new InteractionService(sp.GetRequiredService<DiscordSocketClient>()))
            .AddSingleton<InteractionHandler>()
            .BuildServiceProvider();

        var _ = new ReactionRoleModule(
            _services.GetRequiredService<DiscordSocketClient>(),
            _services.GetRequiredService<IConfiguration>());
    }

    static void Main(string[] args)
        => new Program()
            .RunAsync()
            .GetAwaiter()
            .GetResult();

    private async Task RunAsync()
    {
        var client = _services.GetRequiredService<DiscordSocketClient>();

        client.Log += LogAsync;

        await _services.GetRequiredService<InteractionHandler>()
            .InitializeAsync();

        await client.LoginAsync(TokenType.Bot, _configuration["bot-token"]);
        await client.StartAsync();

        await SetUpPresence(client);
        
        await Task.Delay(Timeout.Infinite);
    }

    private Task LogAsync(LogMessage message)
    {
        Console.WriteLine(message.ToString());
        return Task.CompletedTask;
    }

    private async Task SetUpPresence(DiscordSocketClient client)
    {
        bool isPresenceEnabled = _configuration.GetValue<bool>("presence:enabled");
        if (isPresenceEnabled)
        {
            string gameName = _configuration["presence:game-name"];
            await client.SetGameAsync(gameName);
        }
    }
    
    public static bool IsDebug()
    {
        #if DEBUG
            return true;
        #else
            return false;
        #endif
    }
}