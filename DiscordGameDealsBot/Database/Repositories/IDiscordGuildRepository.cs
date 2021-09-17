using DiscordGameDealsBot.Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordGameDealsBot.Database.Repositories;

public interface IDiscordGuildRepository
{
    Task<ulong> InsertAsync(ulong guildId);
    Task<int> DeleteAsync(ulong guildId);
    Task<IEnumerable<DiscordGuild>> GetAllAsync();
    Task<DiscordGuild> GetByGuildIdAsync(ulong guildId);
}

