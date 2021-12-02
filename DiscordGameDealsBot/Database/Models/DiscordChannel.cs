namespace DiscordGameDealsBot.Database.Models;

public class DiscordChannel
{
    public Guid Id { get; set; }
    public Guid GuildId { get; set; }
    public decimal ChannelId { get; set; }
}