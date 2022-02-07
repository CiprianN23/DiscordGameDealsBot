using Dapper;
using DiscordGameDealsBot.Database.Models;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace DiscordGameDealsBot.Database.Repositories;

internal class DiscordMessageRepository : IDiscordMessageRepository
{
    private readonly IConfigurationRoot _config;

    public DiscordMessageRepository(IConfigurationRoot config)
    {
        _config = config;
    }

    public async Task<int> InsertAsync(decimal messageId, Guid redditPost, Guid channelId)
    {
        await using var _db = new NpgsqlConnection(_config["DISCORD_DBConnection"]);
        await _db.OpenAsync();
        return await _db.ExecuteAsync("INSERT INTO discord_messages (message_id, reddit_post, channel_id) VALUES (@message_id, @reddit_post, @channel_id);", new { @message_id = messageId, @reddit_post = redditPost, @channel_id = channelId });
    }

    public async Task<IEnumerable<DiscordMessage>> GetAllAsync()
    {
        await using var _db = new NpgsqlConnection(_config["DISCORD_DBConnection"]);
        await _db.OpenAsync();
        return await _db.QueryAsync<DiscordMessage>("SELECT * FROM discord_messages;");
    }

    public async Task<int> DeleteAsync(decimal messageId)
    {
        await using var _db = new NpgsqlConnection(_config["DISCORD_DBConnection"]);
        await _db.OpenAsync();
        return await _db.ExecuteAsync("DELETE FROM discord_messages WHERE message_id = @messageId;", new { @messageId = messageId });
    }

    public async Task<DiscordMessage> GetByRedditPostAndChannel(Guid redditPostId, Guid channelId)
    {
        await using var _db = new NpgsqlConnection(_config["DISCORD_DBConnection"]);
        await _db.OpenAsync();
        return await _db.QueryFirstOrDefaultAsync<DiscordMessage>("SELECT * FROM discord_messages WHERE reddit_post = @redditPostId AND channel_id = @channelId;", new { @redditPostId = redditPostId, @channelId = channelId });
    }

    public async Task<IEnumerable<DiscordMessage>> GetAllByChannelAsync(decimal channelId)
    {
        await using var _db = new NpgsqlConnection(_config["DISCORD_DBConnection"]);
        await _db.OpenAsync();
        return await _db.QueryAsync<DiscordMessage>("SELECT * FROM discord_messages WHERE channel_id = (SELECT id FROM discord_channels WHERE channel_id = @channelId);", new { @channelId = channelId });
    }

    public async Task<DiscordMessage> GetByMessageId(decimal messageId)
    {
        await using var _db = new NpgsqlConnection(_config["DISCORD_DBConnection"]);
        await _db.OpenAsync();
        return await _db.QueryFirstOrDefaultAsync<DiscordMessage>("SELECT * FROM discord_messages WHERE message_id = @messageid;", new { @messageid = messageId });
    }
}