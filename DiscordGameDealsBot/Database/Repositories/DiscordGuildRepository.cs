using Dapper;
using DiscordGameDealsBot.Database.Models;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordGameDealsBot.Database.Repositories;

public class DiscordGuildRepository : IDiscordGuildRepository
{
    private readonly IConfigurationRoot _config;
    public DiscordGuildRepository(IConfigurationRoot config)
    {
        _config = config;
    }

    public async Task<ulong> InsertAsync(ulong guildId)
    {
        using var _db = new MySqlConnection(_config.GetConnectionString("Default"));
        await _db.OpenAsync();
        return await _db.ExecuteScalarAsync<ulong>("INSERT IGNORE INTO discord_guilds (guild) VALUES (@guildId) RETURNING id;", new { @guildId = guildId });
    }

    public async Task<int> DeleteAsync(ulong guildId)
    {
        using var _db = new MySqlConnection(_config.GetConnectionString("Default"));
        await _db.OpenAsync();
        return await _db.ExecuteAsync("DELETE FROM discord_guilds WHERE guild = @guildId;", new { @guildId = guildId });
    }

    public async Task<IEnumerable<DiscordGuild>> GetAllAsync()
    {
        using var _db = new MySqlConnection(_config.GetConnectionString("Default"));
        await _db.OpenAsync();
        return await _db.QueryAsync<DiscordGuild>("SELECT * FROM discord_guilds;");
    }

    public async Task<DiscordGuild> GetByGuildIdAsync(ulong guildId)
    {
        using var _db = new MySqlConnection(_config.GetConnectionString("Default"));
        await _db.OpenAsync();
        return await _db.QueryFirstOrDefaultAsync<DiscordGuild>("SELECT * FROM discord_guilds WHERE guild = @guildId;", new { @guildId = guildId });
    }
}

