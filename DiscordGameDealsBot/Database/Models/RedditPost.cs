namespace DiscordGameDealsBot.Database.Models;

public class RedditPost
{
    public Guid Id { get; set; }
    public string? FullName { get; set; }
    public string? PermaLink { get; set; }
}