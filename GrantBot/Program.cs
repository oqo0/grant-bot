using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using GrantBot.Data;
using GrantBot.Modules;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Microsoft.Extensions.Logging;
using Serilog.Events;

namespace GrantBot;

public class Program
{
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _services;
    
    private static string _logLevel = "info";
    
    private readonly DiscordSocketConfig _socketConfig = new()
    {
        GatewayIntents = GatewayIntents.AllUnprivileged,
        AlwaysDownloadUsers = true,
    };

    private Program()
    {
        _configuration = new ConfigurationBuilder()
            .AddYamlFile("config.yml", false, true)
            .AddYamlFile("language.yml", false, true)
            .Build();

        var logLevel = _logLevel.ToLower() switch
        {
            "info" => LogLevel.Information,
            "debug" => LogLevel.Debug,
            "trace" => LogLevel.Trace,
            _ => LogLevel.Error
        };

        _services = new ServiceCollection()
            .AddDbContext<GrantBotDbContext>(
                options => options.UseNpgsql("Host=127.0.0.1;Port=5432;Database=grantbotdatabase;User Id=oqo0;Password=Poi132poi_"))
            .AddSingleton(_configuration)
            .AddSingleton(_socketConfig)
            .AddSingleton<DiscordSocketClient>()
            .AddSingleton(sp => new InteractionService(sp.GetRequiredService<DiscordSocketClient>()))
            .AddSingleton<InteractionHandler>() 
            .AddLogging(conf => conf.AddSerilog().SetMinimumLevel(logLevel))
            .BuildServiceProvider();

        // registering ReactionRoleModule
        var _ = new ReactionRoleModule(
            _services.GetRequiredService<DiscordSocketClient>(),
            _services.GetRequiredService<IConfiguration>(),
            _services.GetRequiredService<ILogger<ReactionRoleModule>>());
    }

    static void Main(string[] args)
    {
        if (args.Length != 0)
            _logLevel = args[0];
        
        Log.Logger = new LoggerConfiguration()
            .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
            .WriteTo.Console()
            .CreateLogger();
        
        new Program()
            .RunAsync()
            .GetAwaiter()
            .GetResult();
    }
    
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

    internal static async Task LogAsync(LogMessage message)
    {
        var severity = message.Severity switch
        {
            LogSeverity.Critical => LogEventLevel.Fatal,
            LogSeverity.Error => LogEventLevel.Error,
            LogSeverity.Warning => LogEventLevel.Warning,
            LogSeverity.Info => LogEventLevel.Information,
            LogSeverity.Verbose => LogEventLevel.Verbose,
            LogSeverity.Debug => LogEventLevel.Debug,
            _ => LogEventLevel.Information
        };
        
        Log.Write(
            severity,
            message.Exception,
            "[{Source}] {Message}",
            message.Source, message.Message);
        
        await Task.CompletedTask;
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