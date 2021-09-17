using DiscordGameDealsBot.Database.Models;
using DiscordGameDealsBot.Database.Repositories;
using DSharpPlus;
using DSharpPlus.EventArgs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordGameDealsBot.Services;

public class DiscordChannelService
{
    private readonly DiscordClient _discordClient;
    private readonly IDiscordGuildRepository _discordGuildRepository;
    private readonly IDiscordMessageRepository _discordMessageRepository;

    public DiscordChannelService(DiscordClient discordClient, IDiscordGuildRepository repository, IDiscordMessageRepository discordMessageRepository)
    {
        _discordClient = discordClient;
        _discordGuildRepository = repository;
        _discordMessageRepository = discordMessageRepository;

        _discordClient.GuildDownloadCompleted += RemoveUnusedGuildsOnStartup;
        _discordClient.GuildDeleted += DeleteGuildOnBotLeave;
        _discordClient.GuildCreated += AddGuildTToDatabaseOnJoin;
        _discordClient.MessageDeleted += RemoveMessageFromDatabaseIfRemoved;
    }

    private async Task RemoveMessageFromDatabaseIfRemoved(DiscordClient sender, MessageDeleteEventArgs e)
    {
        var databaseMessage = await _discordMessageRepository.GetByMessageId(e.Message.Id);
        if (databaseMessage != null)
        {
            await _discordMessageRepository.DeleteAsync(e.Message.Id);
        }
    }

    private async Task AddGuildTToDatabaseOnJoin(DiscordClient sender, GuildCreateEventArgs e)
    {
        await _discordGuildRepository.InsertAsync(e.Guild.Id);
    }

    private async Task DeleteGuildOnBotLeave(DiscordClient sender, GuildDeleteEventArgs e)
    {
        if (e.Unavailable)
            return;

        await DeleteGuildFromDatabase(e.Guild.Id);
    }


    // TODO: Improve lookup and remove
    private async Task RemoveUnusedGuildsOnStartup(DiscordClient sender, GuildDownloadCompletedEventArgs e)
    {
        var databaseGuilds = await _discordGuildRepository.GetAllAsync();

        List<DiscordGuild> guildsFound = new();
        foreach (var guild in e.Guilds)
        {
            var databaseGuild = await _discordGuildRepository.GetByGuildIdAsync(guild.Value.Id);
            if (databaseGuild == null)
                continue;

            guildsFound.Add(databaseGuild);
        }

        List<DiscordGuild> guildsToDelete = new();
        guildsToDelete.AddRange(databaseGuilds.Where(i => !guildsFound.Any(e => i.Guild == e.Guild)));
        foreach (var guildToDelete in guildsToDelete)
        {
            await DeleteGuildFromDatabase(guildToDelete.Guild);
        }

        _discordClient.GuildDownloadCompleted -= RemoveUnusedGuildsOnStartup;
    }

    private async Task DeleteGuildFromDatabase(ulong guildId)
    {
        await _discordGuildRepository.DeleteAsync(guildId);
    }
}

