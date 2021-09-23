using DiscordGameDealsBot.Database.Repositories;
using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.Extensions.Configuration;
using Reddit;
using Reddit.Controllers.EventArgs;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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

        _ = DealExpiredTimer();
    }

    private async void Posts_NewRemoved(object? sender, PostsUpdateEventArgs e)
    {
        var databaseRedditPosts = await _redditPostRepository.GetAllAsync();
        foreach (var removedPost in e.Removed)
        {
            var foundPost = databaseRedditPosts.FirstOrDefault(x => x.Permalink == removedPost.Permalink);
            if (foundPost != null)
            {
                var discordGuildsChannels = await _discordChannelRepository.GetAllAAsync();

                foreach (var databaseChannel in discordGuildsChannels)
                {
                    var channel = await _discordClient.GetChannelAsync(databaseChannel.ChannelId);
                    var databaseMessage = await _discordMessageRepository.GetByRedditPostAndChannel(foundPost.Id, databaseChannel.Id);

                    if (databaseMessage == null)
                        continue;

                    var message = await channel.GetMessageAsync(databaseMessage.MessageId);
                    await message.DeleteAsync();
                    await _discordMessageRepository.DeleteAsync(databaseMessage.MessageId);
                }

                await _redditPostRepository.DeleteAsync(removedPost.Permalink);
            }
        }
    }

    private async Task DealExpiredTimer()
    {
        var timer = new PeriodicTimer(TimeSpan.FromHours(1));
        while (await timer.WaitForNextTickAsync())
        {
            var databaseRedditPosts = await _redditPostRepository.GetAllAsync();

            if (!databaseRedditPosts.Any())
                return;

            foreach (var databasePost in databaseRedditPosts)
            {
                var redditPost = _redditClient.Post(databasePost.Fullname).About();

                if (redditPost == null)
                {
                    await _redditPostRepository.DeleteAsync(databasePost.Permalink);
                    continue;
                }

                if ((!string.IsNullOrEmpty(redditPost.Listing.LinkFlairText) && string.Equals(redditPost.Listing.LinkFlairText, "Expired"))
                    || string.Equals(redditPost.Author, "[deleted]") || redditPost.Removed)
                {
                    var discordGuildsChannels = await _discordChannelRepository.GetAllAAsync();

                    foreach (var databaseChannel in discordGuildsChannels)
                    {
                        var channel = await _discordClient.GetChannelAsync(databaseChannel.ChannelId);
                        var databaseMessage = await _discordMessageRepository.GetByRedditPostAndChannel(databasePost.Id, databaseChannel.Id);

                        if (databaseMessage == null)
                            continue;

                        var message = await channel.GetMessageAsync(databaseMessage.MessageId);
                        await message.DeleteAsync();
                        await _discordMessageRepository.DeleteAsync(databaseMessage.MessageId);
                    }

                    await _redditPostRepository.DeleteAsync(redditPost.Permalink);
                }
            }
        }
    }

    private async void Posts_NewUpdated(object? sender, PostsUpdateEventArgs e)
    {
        foreach (var post in e.Added)
        {
            if (post.Created.Date != DateTime.Now.Date)
                continue;

            bool shouldPostOffer = false;

            // TODO: Change it to per-guild setting
            for (int i = 75; i <= 100; i++)
            {
                if (post.Title.Contains($"{i}%"))
                    shouldPostOffer = true;
            }

            if (post.Title.Contains("Free") || shouldPostOffer)
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
                    var discordChannel = await _discordClient.GetChannelAsync(channel.ChannelId);
                    var message = await _discordClient.SendMessageAsync(discordChannel, embed.Build());
                    await _discordMessageRepository.InsertAsync(message.Id, insertedRedditPost, channel.Id);
                }
            }
        }
    }
}

