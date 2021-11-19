namespace DiscordGameDealsBot.Database.Models;

public class DiscordChannel
{
    public ulong Id { get; set; }
    public ulong GuildId { get; set; }
    public ulong ChannelId { get; set; }
}