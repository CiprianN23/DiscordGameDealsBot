using DiscordGameDealsBot.Database.Models;

namespace DiscordGameDealsBot.Database.Repositories;

public interface IDiscordChannelRepository
{
    Task<int> InsertAsync(Guid guildId, decimal channelId);

    Task<IEnumerable<DiscordChannel>> GetAllAAsync();

    Task<DiscordChannel> GetByGuildAsync(decimal guildId);

    Task<DiscordChannel> GetByChannelId(decimal channelId);

    Task<int> DeleteAsync(decimal channelId);
}