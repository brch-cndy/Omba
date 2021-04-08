using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class requireServerAttribute : CheckBaseAttribute
{
    public ulong allowedServer { get; private set; }

    public requireServerAttribute(ulong serverID)
    {
        allowedServer = serverID;
    }

    public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
    {
        return Task.FromResult(allowedServer == ctx.Guild.Id);
    }
}

