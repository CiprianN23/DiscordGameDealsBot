using DiscordGameDealsBot.Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordGameDealsBot.Database.Repositories;

public interface IDiscordChannelRepository
{
    Task<int> InsertAsync(ulong guildId, ulong channelId);

    Task<IEnumerable<DiscordChannel>> GetAllAAsync();

    Task<DiscordChannel> GetByGuildAsync(ulong guildId);

    Task<DiscordChannel> GetByChannelId(ulong channelId);

    Task<int> DeleteAsync(ulong channelId);
}