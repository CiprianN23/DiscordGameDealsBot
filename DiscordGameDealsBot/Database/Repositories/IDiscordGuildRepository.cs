using DiscordGameDealsBot.Database.Models;

namespace DiscordGameDealsBot.Database.Repositories;

public interface IDiscordGuildRepository
{
    Task<Guid> InsertAsync(decimal guildId);

    Task<int> DeleteAsync(decimal guildId);

    Task<IEnumerable<DiscordGuild>> GetAllAsync();

    Task<DiscordGuild> GetByGuildIdAsync(decimal guildId);
}