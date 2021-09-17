using DiscordGameDealsBot.Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordGameDealsBot.Database.Repositories;

public interface IRedditPostRepository
{
    Task<ulong> InsertAsync(string fullname, string permalink);
    Task<IEnumerable<RedditPost>> GetAllAsync();
    Task<int> DeleteAsync(string permalink);
}