using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Text.Json;

public class Matchmaking : BaseCommandModule
{
    internal Dictionary<ulong, List<string>> queue;

    public Matchmaking()
    {
        queue = new Dictionary<ulong, List<string>>()
        {
            //Borann Boys
            {695448160402931722, new List<string>()},
            //Based Hub
            {836519174472728586, new List<string>()}
        };
    }

    public string convertName(string name)
    {
        //Access Nicknames
        var nicknames = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText("Nicknames.json"));
        //Convert discord name to nickname
        if (nicknames.ContainsKey(name))
            return nicknames[name];
        else
            return name;
    }

    public bool register(ulong server, string name, bool mode)
    {
        if (mode)
        {
            if ((!queue[server].Contains(convertName(name))))
            {
                queue[server].Add(convertName(name));
                return true;
            }
            else
                return false;
        }
        else
        {
            if ((queue[server].Contains(convertName(name))))
            {
                queue[server].Remove(convertName(name));
                return true;
            }
            else
                return false;
        }
    }

    public bool nameset(string name, ulong server, [RemainingText] string nickname)
    {
        var nicknames = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText("Nicknames.json"));
        if (nicknames.ContainsValue(nickname))
            return (true);
        else
        {
            string oldNick = "0";
            if (nicknames.ContainsKey(name))
            {
                oldNick = nicknames[name];
                nicknames.Remove(name);
            }
            nicknames.Add(name, nickname);
            File.WriteAllText("Nicknames.json", JsonSerializer.Serialize<Dictionary<string, string>>(nicknames));
            if (queue[server].Contains(name))
            {
                register(server, name, false);
                register(server, nicknames[name], true);
            }
            else if (queue[server].Contains(oldNick))
            {
                register(server, oldNick, false);
                register(server, nicknames[name], true);
            }
            return (false);
        }
    }

    [Command("setname")]
    [Description("Set your nickname.")]
    [requireChannel("the-matchmaking-channel")]
    [RequireOwner]
    public async Task setName(CommandContext ctx, [RemainingText] string nickname)
    {
        if (nameset(ctx.Message.Author.Mention, ctx.Guild.Id, nickname))
        {
            var embed = new DiscordEmbedBuilder()
                .WithTitle($"Name couldn't be set!")
                .WithColor(new DiscordColor(0xFF55FF))
                .WithDescription($"The nickname of **{nickname}** exists.");
            await ctx.RespondAsync("", embed: embed);
        }
        else if (nameset(ctx.Message.Author.Mention, ctx.Guild.Id, nickname))
        {
            var embed = new DiscordEmbedBuilder()
                .WithTitle($"Name set!")
                .WithColor(new DiscordColor(0xFF55FF))
                .WithDescription($"Set name for **{ctx.Message.Author.Username}** of **{nickname}**.");
            await ctx.RespondAsync("", embed: embed);
        }
    }

    [Command("setnamefor")]
    [Description("Set another users name. Owner only.")]
    [RequireOwner]
    [requireChannel("the-matchmaking-channel")]
    public async Task setNameFor(CommandContext ctx, string name, [RemainingText] string nickname)
    {
        if (nameset(name, ctx.Guild.Id, nickname))
        {
            var embed = new DiscordEmbedBuilder()
                .WithTitle($"Name couldn't be set!")
                .WithColor(new DiscordColor(0xFF55FF))
                .WithDescription($"The nickname of **{nickname}** exists.");
            await ctx.RespondAsync("", embed: embed);
        }
        else if (nameset(name, ctx.Guild.Id, nickname))
        {
            var embed = new DiscordEmbedBuilder()
                .WithTitle($"Name set!")
                .WithColor(new DiscordColor(0xFF55FF))
                .WithDescription($"Set name for **{name}** of **{nickname}**.");
            await ctx.RespondAsync("", embed: embed);
        }
    }

    [Command("register"), Aliases("r", "reg")]
    [Description("Please type ?reg in the matchmaking channel.")]
    [requireChannel("the-matchmaking-channel")]
    public async Task reg(CommandContext ctx, [RemainingText] string name = "")
    {
        if (name == "")
            name = ctx.Message.Author.Mention;
        if (register(ctx.Guild.Id, name, true))
        {
            var embed = new DiscordEmbedBuilder()
                .WithDescription($"**{convertName(name)}** added to queue. **{queue[ctx.Guild.Id].Count}** in queue.")
                .WithColor(new DiscordColor(0xFF55FF));
            await ctx.RespondAsync("", embed: embed);
        }
        else
        {
            var embed = new DiscordEmbedBuilder()
                .WithDescription($"**{convertName(name)}** is already in the queue. **{queue[ctx.Guild.Id].Count}** in queue.")
                .WithColor(new DiscordColor(0xFF55FF));
            await ctx.RespondAsync("", embed: embed);
        }

    }

    [Command("unregister"), Aliases("ur", "ureg")]
    [Description("Remove yourself from the queue.")]
    [requireChannel("the-matchmaking-channel")]
    public async Task ureg(CommandContext ctx, [RemainingText] string name = "")
    {
        if (name == "")
            name = ctx.Message.Author.Mention;
        if (register(ctx.Guild.Id, name, false))
        {
            var embed = new DiscordEmbedBuilder()
                .WithDescription($"**{convertName(name)}** removed from queue. **{queue[ctx.Guild.Id].Count}** in queue.")
                .WithColor(new DiscordColor(0xFF55FF));
            await ctx.RespondAsync("", embed: embed);
        }
        else
        {
            var embed = new DiscordEmbedBuilder()
                .WithDescription($"**{convertName(name)}** is not in the queue. **{queue[ctx.Guild.Id].Count}** in queue.")
                .WithColor(new DiscordColor(0xFF55FF));
            await ctx.RespondAsync("", embed: embed);
        }

    }

    [Command("clear"), Aliases("c")]
    [Description("Clear the queue.")]
    [requireChannel("the-matchmaking-channel")]
    public async Task clear(CommandContext ctx)
    {
        queue[ctx.Guild.Id].Clear();
        var embed = new DiscordEmbedBuilder()
                .WithTitle($"Queue cleared!")
                .WithDescription($"**{ctx.Message.Author.Username}** cleared the queue.")
                .WithColor(new DiscordColor(0xFF55FF));
        await ctx.RespondAsync("", embed: embed);
    }

    [Command("queue"), Aliases("q", "show")]
    [Description("Display the queue.")]
    [requireChannel("the-matchmaking-channel")]
    public async Task displayQueue(CommandContext ctx)
    {
        if (queue[ctx.Guild.Id].Count == 0)
        {
            var embed = new DiscordEmbedBuilder()
                .WithDescription($"There is no one in the queue.")
                .WithColor(new DiscordColor(0xFF55FF));
            await ctx.RespondAsync("", embed: embed);
        }
        else if (queue[ctx.Guild.Id].Count == 1)
        {
            var embed = new DiscordEmbedBuilder()
                .WithTitle($"There is {queue[ctx.Guild.Id].Count()} person in queue.")
                .WithColor(new DiscordColor(0xFF55FF))
                .WithDescription($"{string.Join("\n", queue[ctx.Guild.Id])}");
            await ctx.RespondAsync("", embed: embed);
        }
        else
        {
            var embed = new DiscordEmbedBuilder()
                .WithTitle($"There are {queue[ctx.Guild.Id].Count()} people in queue.")
                .WithColor(new DiscordColor(0xFF55FF))
                .WithDescription($"{string.Join("\n", queue[ctx.Guild.Id])}");
            await ctx.RespondAsync("", embed: embed);
        }
    }

    [Command("roll"), Aliases("rr", "reroll")]
    [Description("Roll a fight.")]
    [requireChannel("the-matchmaking-channel")]
    public async Task roll(CommandContext ctx)
    {
        var images = JsonSerializer.Deserialize<List<string>>(File.ReadAllText("footerImages.json"));
        List<string> team1 = new List<string>(); List<string> team2 = new List<string>(); Random random = new Random();
        if (queue[ctx.Guild.Id].Count == 0 || queue[ctx.Guild.Id].Count == 1)
        {
            var embed = new DiscordEmbedBuilder().WithColor(new DiscordColor(0xFF55FF)).WithTitle($"There aren't enough people to roll a fight.");
            await ctx.RespondAsync("", embed: embed);
        } else
        {
            List<string> teams = queue[ctx.Guild.Id].ToList();
            for (int i = teams.Count; i > 0; i--)
            {
                var randomIndex = random.Next(teams.Count);

                if (i % 2 != 0)
                {
                    team1.Add(teams[randomIndex]);
                }
                else
                {
                    team2.Add(teams[randomIndex]);
                }
                teams.RemoveAt(randomIndex);
            }
            var embed = new DiscordEmbedBuilder()
                .WithTitle($"{team1.Count()}v{team2.Count()} prepared!")
                .WithColor(new DiscordColor(0xFF55FF))
                .WithThumbnail($"{ctx.Guild.IconUrl}")
                .WithImageUrl(images[random.Next(images.Count)])
                .AddField($"Team 1", $"{string.Join("\n", team1)}", true)
                .WithFooter(DateTime.Now.ToString(), "https://cdn.discordapp.com/attachments/838143199289278534/838143225134579712/breach_candy.png")
                .AddField($"Team 2", $"{string.Join("\n", team2)}", true);
            await ctx.RespondAsync("", embed: embed);
            team1.Clear(); team2.Clear();
        }

    }
}
