using DiscordGameDealsBot.Commands;
using DiscordGameDealsBot.Database.Repositories;
using DiscordGameDealsBot.Services;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordGameDealsBot;

public static class Program
{
    private static IConfigurationRoot? _config;

    public static async Task Main()
    {
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

        //Create the configurationNo service for type 'DiscordGameDealsBota
        var _builder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile(path: "appsettings.json", optional: false, reloadOnChange: true);

        _config = _builder.Build();

        var discordClient = new DiscordClient(new DiscordConfiguration()
        {
            Token = Environment.GetEnvironmentVariable("DISCORD_TOKEN"),
            TokenType = TokenType.Bot,
            AutoReconnect = true,
            AlwaysCacheMembers = false
        });

        var services = new ServiceCollection()
            .AddSingleton(_config)
            .AddSingleton(discordClient)
            .AddSingleton<RedditService>()
            .AddSingleton<DiscordChannelService>()
            .AddTransient<IRedditPostRepository, RedditPostRepository>()
            .AddTransient<IDiscordGuildRepository, DiscordGuildRepository>()
            .AddTransient<IDiscordMessageRepository, DiscordMessageRepository>()
            .AddTransient<IDiscordChannelRepository, DiscordChannelRepository>();

        var serviceProvider = services.BuildServiceProvider();

        var ccfg = new CommandsNextConfiguration
        {
            Services = serviceProvider,

            // enable mentioning the bot as a command prefix
            EnableMentionPrefix = true
        };

        var commands = discordClient.UseCommandsNext(ccfg);
        commands.RegisterCommands<BotCommands>();

        //Start the bot
        await discordClient.ConnectAsync();

        serviceProvider.GetRequiredService<DiscordChannelService>();
        serviceProvider.GetRequiredService<RedditService>();

        await Task.Delay(-1);
    }
}