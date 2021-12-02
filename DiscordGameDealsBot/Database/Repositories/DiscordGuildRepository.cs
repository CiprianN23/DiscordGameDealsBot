using Dapper;
using DiscordGameDealsBot.Database.Models;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace DiscordGameDealsBot.Database.Repositories;

public class DiscordGuildRepository : IDiscordGuildRepository
{
    private readonly IConfigurationRoot _config;

    public DiscordGuildRepository(IConfigurationRoot config)
    {
        _config = config;
    }

    public async Task<Guid> InsertAsync(decimal guildId)
    {
        await using var _db = new NpgsqlConnection(_config.GetConnectionString("Default"));
        await _db.OpenAsync();
        return await _db.ExecuteScalarAsync<Guid>("INSERT INTO discord_guilds (guild) VALUES (@guildId) ON CONFLICT (guild) DO NOTHING RETURNING id;", new { @guildId = guildId });
    }

    public async Task<int> DeleteAsync(decimal guildId)
    {
        await using var _db = new NpgsqlConnection(_config.GetConnectionString("Default"));
        await _db.OpenAsync();
        return await _db.ExecuteAsync("DELETE FROM discord_guilds WHERE guild = @guildId;", new { @guildId = guildId });
    }

    public async Task<IEnumerable<DiscordGuild>> GetAllAsync()
    {
        await using var _db = new NpgsqlConnection(_config.GetConnectionString("Default"));
        await _db.OpenAsync();
        return await _db.QueryAsync<DiscordGuild>("SELECT * FROM discord_guilds;");
    }

    public async Task<DiscordGuild> GetByGuildIdAsync(decimal guildId)
    {
        await using var _db = new NpgsqlConnection(_config.GetConnectionString("Default"));
        await _db.OpenAsync();
        return await _db.QueryFirstOrDefaultAsync<DiscordGuild>("SELECT * FROM discord_guilds WHERE guild = @guildId;", new { @guildId = guildId });
    }
}