using DiscordGameDealsBot.Database.Repositories;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using Microsoft.Extensions.Configuration;
using Reddit;
using Reddit.Controllers.EventArgs;

namespace DiscordGameDealsBot.Services;

public class RedditService
{
    private readonly IConfigurationRoot _config;
    private readonly RedditClient _redditClient;
    private readonly DiscordClient _discordClient;

    private readonly IRedditPostRepository _redditPostRepository;
    private readonly IDiscordChannelRepository _discordChannelRepository;
    private readonly IDiscordMessageRepository _discordMessageRepository;

    public RedditService(IConfigurationRoot config, DiscordClient discordClient, IDiscordMessageRepository discordMessageRepository, IDiscordChannelRepository discordChannelRepository, IRedditPostRepository redditPostRepository)
    {
        _config = config;
        _discordClient = discordClient;
        _discordMessageRepository = discordMessageRepository;
        _discordChannelRepository = discordChannelRepository;
        _redditPostRepository = redditPostRepository;

        _redditClient = new RedditClient(appId: _config.GetSection("Reddit:AppId").Value, appSecret: _config.GetSection("Reddit:AppSecret").Value, refreshToken: _config.GetSection("Reddit:RefreshToken").Value);

        var subReddit = _redditClient.Subreddit(_config.GetSection("Reddit:Subreddit").Value);
        subReddit.Posts.GetNew();
        subReddit.Posts.MonitorNew();
        subReddit.Posts.NewUpdated += Posts_NewUpdated;
        subReddit.Posts.NewUpdated += Posts_NewRemoved;

        _ = DeleteDealsIfTheyAreExpiredOrRemoved();
    }

    private async void Posts_NewRemoved(object? sender, PostsUpdateEventArgs e)
    {
        Console.WriteLine($"{DateTimeOffset.Now} New post added/removed.");

        var databaseRedditPosts = await _redditPostRepository.GetAllAsync();
        foreach (var removedPost in e.Removed)
        {
            var foundPost = databaseRedditPosts.FirstOrDefault(x => x.PermaLink == removedPost.Permalink);
            if (foundPost != null)
            {
                var discordGuildsChannels = await _discordChannelRepository.GetAllAAsync();

                foreach (var databaseChannel in discordGuildsChannels)
                {
                    var channel = await _discordClient.GetChannelAsync(decimal.ToUInt64(databaseChannel.ChannelId));
                    var databaseMessage = await _discordMessageRepository.GetByRedditPostAndChannel(foundPost.Id, databaseChannel.Id);

                    if (databaseMessage == null)
                        continue;

                    var message = await channel.GetMessageAsync(decimal.ToUInt64(databaseMessage.MessageId));
                    await message.DeleteAsync();
                    await _discordMessageRepository.DeleteAsync(decimal.ToUInt64(databaseMessage.MessageId));
                }

                await _redditPostRepository.DeleteAsync(removedPost.Permalink);
            }
        }

        await DeleteDealsIfTheyAreExpiredOrRemoved();
    }

    private async Task DeleteDealsIfTheyAreExpiredOrRemoved()
    {
        Console.WriteLine($"{DateTimeOffset.Now} Deal expired clean is running.");

        var databaseRedditPosts = await _redditPostRepository.GetAllAsync();

        if (!databaseRedditPosts.Any())
            return;

        foreach (var databasePost in databaseRedditPosts)
        {
            var redditPost = _redditClient.Post(databasePost.FullName).About();

            if (redditPost == null)
            {
                ArgumentNullException.ThrowIfNull(databasePost.PermaLink);
                await _redditPostRepository.DeleteAsync(databasePost.PermaLink);
                continue;
            }

            if ((!string.IsNullOrEmpty(redditPost.Listing.LinkFlairText) && string.Equals(redditPost.Listing.LinkFlairText, "Expired"))
                || string.Equals(redditPost.Author, "[deleted]") || redditPost.Removed)
            {
                var discordGuildsChannels = await _discordChannelRepository.GetAllAAsync();

                foreach (var databaseChannel in discordGuildsChannels)
                {
                    var channel = await _discordClient.GetChannelAsync(decimal.ToUInt64(databaseChannel.ChannelId));
                    var databaseMessage = await _discordMessageRepository.GetByRedditPostAndChannel(databasePost.Id, databaseChannel.Id);

                    if (databaseMessage == null)
                        continue;

                    try
                    {
                        var message = await channel.GetMessageAsync(decimal.ToUInt64(databaseMessage.MessageId));
                        await message.DeleteAsync();
                    }
                    catch (NotFoundException ex)
                    {
                        Console.WriteLine($"Message was not found to delete inside deal expire timer. Message Id: {databaseMessage.MessageId}");
                        Console.WriteLine(ex.Message);
                    }
                    finally
                    {
                        await _discordMessageRepository.DeleteAsync(databaseMessage.MessageId);
                    }
                }

                await _redditPostRepository.DeleteAsync(redditPost.Permalink);
            }
        }
    }

    private async void Posts_NewUpdated(object? sender, PostsUpdateEventArgs e)
    {
        foreach (var post in e.Added)
        {
            if (post.Created.Date != DateTime.Now.Date)
                continue;

            if (post.Title.Contains("Free") || post.Title.Contains("100%"))
            {
                var embed = new DiscordEmbedBuilder();
                embed.AddField("Reddit post", $"[Click to open](https://reddit.com{post.Permalink})")
                    .WithDescription(post.Title)
                    .WithTimestamp(post.Created.ToUniversalTime())
                    .WithColor(DiscordColor.Green);

                var insertedRedditPost = await _redditPostRepository.InsertAsync(post.Fullname, post.Permalink);

                var guildChannels = await _discordChannelRepository.GetAllAAsync();

                foreach (var channel in guildChannels)
                {
                    var discordChannel = await _discordClient.GetChannelAsync(decimal.ToUInt64(channel.ChannelId));
                    var message = await _discordClient.SendMessageAsync(discordChannel, embed.Build());
                    await _discordMessageRepository.InsertAsync(message.Id, insertedRedditPost, channel.Id);
                }
            }
        }
    }
}