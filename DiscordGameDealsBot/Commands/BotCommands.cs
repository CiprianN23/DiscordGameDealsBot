using DiscordGameDealsBot.Database.Repositories;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace DiscordGameDealsBot.Commands;

public class BotCommands : BaseCommandModule
{
    private readonly IDiscordGuildRepository _discordGuildRepository;
    private readonly IDiscordChannelRepository _discordChannelRepository;
    private readonly IDiscordMessageRepository _discordMessageRepository;

    public BotCommands(IDiscordGuildRepository discordGuildRepository, IDiscordChannelRepository discordChannelRepository, IDiscordMessageRepository discordMessageRepository)
    {
        _discordGuildRepository = discordGuildRepository;
        _discordChannelRepository = discordChannelRepository;
        _discordMessageRepository = discordMessageRepository;
    }

    [Command("plsdealshere")]
    [Description("Save channel to post deals into.")]
    [Hidden]
    [RequireUserPermissions(DSharpPlus.Permissions.None)]
    public async Task SaveChannel(CommandContext ctx)
    {
        await ctx.TriggerTypingAsync();

        var channelAreadyExists = await _discordChannelRepository.GetByGuildAsync(ctx.Guild.Id);
        if (channelAreadyExists != null)
        {
            await ctx.RespondAsync("This server has already a channel registered!");
            return;
        }

        var databaseGuild = await _discordGuildRepository.GetByGuildIdAsync(ctx.Guild.Id);
        Guid? databaseGuildId = databaseGuild?.Id;

        if (databaseGuildId == null)
        {
            databaseGuildId = await _discordGuildRepository.InsertAsync(ctx.Guild.Id);
        }

        await _discordChannelRepository.InsertAsync(databaseGuildId.Value, ctx.Channel.Id);

        await ctx.RespondAsync("Game deals channel saved successfully!");
    }

    [Command("plsremovedeals")]
    [Description("Stop the bot from posting deals and deletes all posted deals.")]
    [Hidden]
    [RequireUserPermissions(DSharpPlus.Permissions.None)]
    public async Task DeleteChannel(CommandContext ctx)
    {
        await ctx.TriggerTypingAsync();

        var messages = await _discordMessageRepository.GetAllByChannelAsync(ctx.Channel.Id);
        if (messages.Any())
        {
            foreach (var message in messages)
            {
                var discordMessage = await ctx.Channel.GetMessageAsync(decimal.ToUInt64(message.MessageId));

                if (discordMessage == null)
                    continue;

                await discordMessage.DeleteAsync();
            }
        }

        await _discordChannelRepository.DeleteAsync(ctx.Channel.Id);

        await ctx.RespondAsync("Game deals channel removed successfully!");
    }
}