using Dapper;
using DiscordGameDealsBot.Database.Models;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordGameDealsBot.Database.Repositories;

internal class DiscordMessageRepository : IDiscordMessageRepository
{
    private readonly IConfigurationRoot _config;

    public DiscordMessageRepository(IConfigurationRoot config)
    {
        _config = config;
    }

    public async Task<int> InsertAsync(ulong messageId, ulong redditPost, ulong channelId)
    {
        await using var _db = new MySqlConnection(_config.GetConnectionString("Default"));
        await _db.OpenAsync();
        return await _db.ExecuteAsync("INSERT INTO discord_messages (messageid, redditpost, channelid) VALUES (@message_id, @reddit_post, @channel_id);", new { @message_id = messageId, @reddit_post = redditPost, @channel_id = channelId });
    }

    public async Task<IEnumerable<DiscordMessage>> GetAllAsync()
    {
        await using var _db = new MySqlConnection(_config.GetConnectionString("Default"));
        await _db.OpenAsync();
        return await _db.QueryAsync<DiscordMessage>("SELECT * FROM discord_messages;");
    }

    public async Task<int> DeleteAsync(ulong messageId)
    {
        await using var _db = new MySqlConnection(_config.GetConnectionString("Default"));
        await _db.OpenAsync();
        return await _db.ExecuteAsync("DELETE FROM discord_messages WHERE messageid = @messageId;", new { @messageId = messageId });
    }

    public async Task<DiscordMessage> GetByRedditPostAndChannel(ulong redditPostId, ulong channelId)
    {
        await using var _db = new MySqlConnection(_config.GetConnectionString("Default"));
        await _db.OpenAsync();
        return await _db.QueryFirstOrDefaultAsync<DiscordMessage>("SELECT * FROM discord_messages WHERE redditpost = @redditPostId AND channelid = @channelId;", new { @redditPostId = redditPostId, @channelId = channelId });
    }

    public async Task<IEnumerable<DiscordMessage>> GetAllByChannelAsync(ulong channelId)
    {
        await using var _db = new MySqlConnection(_config.GetConnectionString("Default"));
        await _db.OpenAsync();
        return await _db.QueryAsync<DiscordMessage>("SELECT * FROM discord_messages WHERE channelid = (SELECT id FROM discord_channels WHERE channelid = @channelId);", new { @channelId = channelId });
    }

    public async Task<DiscordMessage> GetByMessageId(ulong messageId)
    {
        await using var _db = new MySqlConnection(_config.GetConnectionString("Default"));
        await _db.OpenAsync();
        return await _db.QueryFirstOrDefaultAsync<DiscordMessage>("SELECT * FROM discord_messages WHERE messageid = @messageid;", new { @messageid = messageId });
    }
}