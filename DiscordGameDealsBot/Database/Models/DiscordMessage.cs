namespace DiscordGameDealsBot.Database.Models;

public class DiscordMessage
{
    public ulong MessageId { get; set; }
    public ulong RedditPost { get; set; }
    public ulong ChannelId { get; set; }
}

