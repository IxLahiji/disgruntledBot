using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace disgruntledBot.Modules
{
    [Name("Moderator")]
    [RequireContext(ContextType.Guild)]
    public class Moderation : ModuleBase<SocketCommandContext>
    {
        [Command("adminTest")]
        [Summary("Checks to see if you're an administrator.")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task AdminTest()
        {
            await ReplyAsync($"You are an admin :smile:");
        }

        [Command("userRoles")]
        [Summary("Returns the users Role")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task UserRoles([Remainder]SocketGuildUser user)
        {
            await ReplyAsync($"Roles are {user.Roles.ToString()}.");
        }
    }
}
