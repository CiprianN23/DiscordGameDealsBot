using Dapper;
using DiscordGameDealsBot.Database.Models;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordGameDealsBot.Database.Repositories;

public class DiscordChannelRepository : IDiscordChannelRepository
{
    private readonly IConfigurationRoot _config;
    public DiscordChannelRepository(IConfigurationRoot config)
    {
        _config = config;
    }

    public async Task<int> InsertAsync(ulong guildId, ulong channelId)
    {
        using var _db = new MySqlConnection(_config.GetConnectionString("Default"));
        await _db.OpenAsync();
        return await _db.ExecuteAsync("INSERT INTO discord_channels (guildid, channelid) VALUES (@guildid, @channelid);", new { @guildid = guildId, @channelid = channelId });
    }

    public async Task<IEnumerable<DiscordChannel>> GetAllAAsync()
    {
        using var _db = new MySqlConnection(_config.GetConnectionString("Default"));
        await _db.OpenAsync();
        return await _db.QueryAsync<DiscordChannel>("SELECT * FROM discord_channels;");
    }

    public async Task<DiscordChannel> GetByGuildAsync(ulong guildId)
    {
        using var _db = new MySqlConnection(_config.GetConnectionString("Default"));
        await _db.OpenAsync();
        return await _db.QueryFirstOrDefaultAsync<DiscordChannel>("SELECT * FROM discord_channels WHERE guildid = (SELECT id FROM discord_guilds WHERE guild = @guildId);", new { @guildId = guildId });
    }

    public async Task<DiscordChannel> GetByChannelId(ulong channelId)
    {
        using var _db = new MySqlConnection(_config.GetConnectionString("Default"));
        await _db.OpenAsync();
        return await _db.QueryFirstOrDefaultAsync<DiscordChannel>("SELECT * FROM discord_channels WHERE channelid = @channelId;", new { @channelId = channelId });
    }

    public async Task<int> DeleteAsync(ulong channelId)
    {
        using var _db = new MySqlConnection(_config.GetConnectionString("Default"));
        await _db.OpenAsync();
        return await _db.ExecuteAsync("DELETE FROM discord_channels WHERE channelid = @channelId;", new { @channelId = channelId });
    }
}

