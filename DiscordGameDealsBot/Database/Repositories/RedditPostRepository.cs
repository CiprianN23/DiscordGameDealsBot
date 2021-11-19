using Dapper;
using DiscordGameDealsBot.Database.Models;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordGameDealsBot.Database.Repositories;

public class RedditPostRepository : IRedditPostRepository
{
    private readonly IConfigurationRoot _config;

    public RedditPostRepository(IConfigurationRoot config)
    {
        _config = config;
    }

    public async Task<ulong> InsertAsync(string fullname, string permalink)
    {
        await using var _db = new MySqlConnection(_config.GetConnectionString("Default"));
        await _db.OpenAsync();
        return await _db.ExecuteScalarAsync<ulong>("INSERT INTO reddit_posts (fullname, permalink) VALUES (@fullname, @permalink) RETURNING Id;", new { @fullname = fullname, @permalink = permalink });
    }

    public async Task<IEnumerable<RedditPost>> GetAllAsync()
    {
        await using var _db = new MySqlConnection(_config.GetConnectionString("Default"));
        await _db.OpenAsync();
        return await _db.QueryAsync<RedditPost>("SELECT id, fullname, permalink FROM reddit_posts;");
    }

    public async Task<int> DeleteAsync(string permalink)
    {
        await using var _db = new MySqlConnection(_config.GetConnectionString("Default"));
        await _db.OpenAsync();
        return await _db.ExecuteAsync("DELETE FROM reddit_posts WHERE permalink = @permalink;", new { @permalink = permalink });
    }
}