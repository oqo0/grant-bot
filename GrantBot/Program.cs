﻿using Discord;
using Discord.Addons.Hosting;
using Discord.Commands;
using Discord.WebSocket;
using GrantBot.Data;
using GrantBot.Modules;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .WriteTo.Console()
    .CreateLogger();

try
{
    Log.Information("Starting host");
    
    var builder = Host.CreateDefaultBuilder();
    
    builder.UseSerilog();
    
    #region Configure configuration

    builder.ConfigureAppConfiguration(options =>
    {
        options.AddYamlFile("config.yml", false, true);
        options.AddYamlFile("language.yml", false, true);
    });

    #endregion
    
    #region Configure Discord Bot Host

    builder.ConfigureDiscordHost((context, config) =>
    {
        config.SocketConfig = new DiscordSocketConfig
        {
            LogLevel = context.Configuration.GetValue<LogSeverity>("log-severity"),
            AlwaysDownloadUsers = true,
            MessageCacheSize = 200
        };

        config.Token = context.Configuration["bot-token"];
    });

    #endregion

    #region Configure Interation handling

    builder.UseCommandService((context, config) =>
    {
        config.DefaultRunMode = RunMode.Async;
        config.CaseSensitiveCommands = false;
    });
    
    builder.UseInteractionService((context, config) =>
    {
        config.LogLevel = context.Configuration.GetValue<LogSeverity>("log-severity");
        config.UseCompiledLambda = true;
    });

    #endregion

    #region Configure Services
    
    builder.ConfigureServices((context, services) =>
    {
        services.AddDbContext<GrantBotDbContext>(options =>
        {
            options.UseNpgsql(context.Configuration["db-connection-string"]);
        });
        services.AddHostedService<PlayingGameModule>();
        services.AddHostedService<ReactionRoleModule>();
    });

    #endregion

    var host = builder.Build();
    
    await host.RunAsync();
    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}