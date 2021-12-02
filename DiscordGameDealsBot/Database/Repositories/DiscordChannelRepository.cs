using Dapper;
using DiscordGameDealsBot.Database.Models;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace DiscordGameDealsBot.Database.Repositories;

public class DiscordChannelRepository : IDiscordChannelRepository
{
    private readonly IConfigurationRoot _config;

    public DiscordChannelRepository(IConfigurationRoot config)
    {
        _config = config;
    }

    public async Task<int> InsertAsync(Guid guildId, decimal channelId)
    {
        await using var _db = new NpgsqlConnection(_config.GetConnectionString("Default"));
        await _db.OpenAsync();
        return await _db.ExecuteAsync("INSERT INTO discord_channels (guild_id, channel_id) VALUES (@guildid, @channelid);", new { @guildid = guildId, @channelid = channelId });
    }

    public async Task<IEnumerable<DiscordChannel>> GetAllAAsync()
    {
        await using var _db = new NpgsqlConnection(_config.GetConnectionString("Default"));
        await _db.OpenAsync();
        return await _db.QueryAsync<DiscordChannel>("SELECT * FROM discord_channels;");
    }

    public async Task<DiscordChannel> GetByGuildAsync(decimal guildId)
    {
        await using var _db = new NpgsqlConnection(_config.GetConnectionString("Default"));
        await _db.OpenAsync();
        return await _db.QueryFirstOrDefaultAsync<DiscordChannel>("SELECT * FROM discord_channels WHERE guild_id = (SELECT id FROM discord_guilds WHERE guild = @guildId);", new { @guildId = guildId });
    }

    public async Task<DiscordChannel> GetByChannelId(decimal channelId)
    {
        await using var _db = new NpgsqlConnection(_config.GetConnectionString("Default"));
        await _db.OpenAsync();
        return await _db.QueryFirstOrDefaultAsync<DiscordChannel>("SELECT * FROM discord_channels WHERE channel_id = @channelId;", new { @channelId = channelId });
    }

    public async Task<int> DeleteAsync(decimal channelId)
    {
        await using var _db = new NpgsqlConnection(_config.GetConnectionString("Default"));
        await _db.OpenAsync();
        return await _db.ExecuteAsync("DELETE FROM discord_channels WHERE channel_id = @channelId;", new { @channelId = channelId });
    }
}