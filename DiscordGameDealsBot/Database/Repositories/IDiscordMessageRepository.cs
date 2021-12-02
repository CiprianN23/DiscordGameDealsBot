using DiscordGameDealsBot.Database.Models;

namespace DiscordGameDealsBot.Database.Repositories;

public interface IDiscordMessageRepository
{
    Task<int> InsertAsync(decimal messageId, Guid redditPost, Guid channelId);

    Task<IEnumerable<DiscordMessage>> GetAllAsync();

    Task<int> DeleteAsync(decimal messageId);

    Task<DiscordMessage> GetByRedditPostAndChannel(Guid redditPostId, Guid channelId);

    Task<IEnumerable<DiscordMessage>> GetAllByChannelAsync(decimal channelId);

    Task<DiscordMessage> GetByMessageId(decimal messageId);
}