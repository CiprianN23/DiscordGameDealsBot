namespace DiscordGameDealsBot.Database.Models;

public class DiscordMessage
{
    public Guid Id { get; set; }
    public decimal MessageId { get; set; }
    public Guid RedditPost { get; set; }
    public Guid ChannelId { get; set; }
}