using Dapper;
using DiscordGameDealsBot.Database.Models;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace DiscordGameDealsBot.Database.Repositories;

public class RedditPostRepository : IRedditPostRepository
{
    private readonly IConfigurationRoot _config;

    public RedditPostRepository(IConfigurationRoot config)
    {
        _config = config;
    }

    public async Task<Guid> InsertAsync(string fullname, string permalink)
    {
        await using var _db = new NpgsqlConnection(_config["DISCORD_DBConnection"]);
        await _db.OpenAsync();
        return await _db.ExecuteScalarAsync<Guid>("INSERT INTO reddit_posts (full_name, perma_link) VALUES (@fullname, @permalink) RETURNING Id;", new { @fullname = fullname, @permalink = permalink });
    }

    public async Task<IEnumerable<RedditPost>> GetAllAsync()
    {
        await using var _db = new NpgsqlConnection(_config["DISCORD_DBConnection"]);
        await _db.OpenAsync();
        return await _db.QueryAsync<RedditPost>("SELECT * FROM reddit_posts;");
    }

    public async Task<int> DeleteAsync(string permalink)
    {
        await using var _db = new NpgsqlConnection(_config["DISCORD_DBConnection"]);
        await _db.OpenAsync();
        return await _db.ExecuteAsync("DELETE FROM reddit_posts WHERE perma_link = @permalink;", new { @permalink = permalink });
    }
}