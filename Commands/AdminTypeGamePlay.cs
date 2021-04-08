
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Text.Json;
public class AdminTypeGamePlay : BaseCommandModule
{
    [Command("purge")]
    [RequireBotPermissions(DSharpPlus.Permissions.ManageMessages)]
    [RequireUserPermissions(DSharpPlus.Permissions.ManageMessages)]
    public async Task purge(CommandContext ctx, int amount)
    {
        // Check if the amount provided by the user is positive.
        if (amount <= 0)
        {
            var embed = new DiscordEmbedBuilder
            {
                Title = $"Unable to complete.",
                Description = $"Number of messages provided must be postitive.",
                Color = new DiscordColor(0xFF55FF) 
            };
            await ctx.RespondAsync("", embed: embed);
            return;
        }
        var messages = await ctx.Channel.GetMessagesAsync(amount + 1);

        var filteredMessages = messages.Where(x => (DateTimeOffset.UtcNow - x.Timestamp).TotalDays <= 1);

        // Get the total amount of messages.
        var count = filteredMessages.Count();

        // Check if there are any messages to delete.
        if (count == 0)
            await ctx.RespondAsync("Nothing to delete.");
        else
        {
            await ctx.Channel.DeleteMessagesAsync(filteredMessages, $"Cleared by {ctx.Message.Author.Username}");
            var embed = new DiscordEmbedBuilder
            {
                Title = $"Done.",
                Description = $"Removed **{count - 1}** messages from the past day.",
                Color = new DiscordColor(0xFF55FF) 
            };
            await ctx.RespondAsync("", embed: embed);
        }
    }

    [Command("mute")]
    [RequireBotPermissions(DSharpPlus.Permissions.ManageRoles)]
    [RequireUserPermissions(DSharpPlus.Permissions.ManageRoles)]
    [RequireGuild()]
    [requireServer(695448160402931722)]
    public async Task mute(CommandContext ctx, DiscordMember member)
    {
        DiscordMember pedophile = member;
        if (pedophile.Roles.Contains(ctx.Guild.GetRole(695834582850732114)) || pedophile.Roles.Contains(ctx.Guild.GetRole(726311511768694785)) || pedophile.IsBot)
        {
            var embed = new DiscordEmbedBuilder
            {
                Title = $"Unable to complete.",
                Description = $"**{member.Username}** cannot be muted.",
                Color = new DiscordColor(0xFF55FF) 
            };
            await ctx.RespondAsync("", embed: embed);
        }
        else if (pedophile.Roles.Contains(ctx.Guild.GetRole(721613243239497748)))
        {
            var embed = new DiscordEmbedBuilder
            {
                Title = $"Unable to complete.",
                Description = $"**{member.Username}** is already muted.",
                Color = new DiscordColor(0xFF55FF) 
            };
            await ctx.RespondAsync("", embed: embed);
        }
        else
        {
            await pedophile.GrantRoleAsync(ctx.Guild.GetRole(721613243239497748), $"Muted by {ctx.Message.Author.Username}");
            var embed = new DiscordEmbedBuilder
            {
                Title = $"Done.",
                Description = $"**{member.Username}** was muted.",
                Color = new DiscordColor(0xFF55FF) 
            };
            await ctx.RespondAsync("", embed: embed);
        }

    }
    [Command("unmute")]
    [RequireBotPermissions(DSharpPlus.Permissions.ManageRoles)]
    [RequireUserPermissions(DSharpPlus.Permissions.ManageRoles)]
    [RequireGuild()]
    [requireServer(695448160402931722)]
    public async Task unmute(CommandContext ctx, DiscordMember member)
    {
        DiscordMember pedophile = member;
        if (pedophile.Roles.Contains(ctx.Guild.GetRole(721613243239497748)))
        {
            await pedophile.RevokeRoleAsync(ctx.Guild.GetRole(721613243239497748), $"Unmuted by {ctx.Message.Author.Username}");
            var embed = new DiscordEmbedBuilder
            {
                Title = $"Done.",
                Description = $"**{member.Username}** was unmuted.",
                Color = new DiscordColor(0xFF55FF) 
            };
            await ctx.RespondAsync("", embed: embed);

        }
        else
        {
            var embed1 = new DiscordEmbedBuilder
            {
                Title = $"Unable to complete.",
                Description = $"**{member.Username}** is not muted.",
                Color = new DiscordColor(0xFF55FF) 
            };
            await ctx.RespondAsync("", embed: embed1);
        }

    }

    [Command("addimage")]
    [RequireOwner]
    public async Task addImage(CommandContext ctx, [RemainingText] string value)
    {
        var images = JsonSerializer.Deserialize<List<string>>(File.ReadAllText("footerImages.json"));
        images.Add(value);
        File.WriteAllText("footerImages.json", JsonSerializer.Serialize<List<string>>(images));
        var embed = new DiscordEmbedBuilder()
                .WithTitle($"Image added!")
                .WithColor(new DiscordColor(0xFF55FF))
                .WithImageUrl(value);
        await ctx.RespondAsync("", embed: embed);
    }

}