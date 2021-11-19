using DiscordGameDealsBot.Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordGameDealsBot.Database.Repositories;

public interface IDiscordMessageRepository
{
    Task<int> InsertAsync(ulong messageId, ulong redditPost, ulong channelId);

    Task<IEnumerable<DiscordMessage>> GetAllAsync();

    Task<int> DeleteAsync(ulong messageId);

    Task<DiscordMessage> GetByRedditPostAndChannel(ulong redditPostId, ulong channelId);

    Task<IEnumerable<DiscordMessage>> GetAllByChannelAsync(ulong channelId);

    Task<DiscordMessage> GetByMessageId(ulong messageId);
}