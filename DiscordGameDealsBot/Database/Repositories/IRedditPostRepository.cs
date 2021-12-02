using DiscordGameDealsBot.Database.Models;

namespace DiscordGameDealsBot.Database.Repositories;

public interface IRedditPostRepository
{
    Task<Guid> InsertAsync(string fullname, string permalink);

    Task<IEnumerable<RedditPost>> GetAllAsync();

    Task<int> DeleteAsync(string permalink);
}