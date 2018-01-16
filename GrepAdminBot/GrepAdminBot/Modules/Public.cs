using Discord;
using Discord.Commands;
using Discord.WebSocket;

using System;
using System.Threading.Tasks;

namespace GrepAdminBot.Modules
{
    [Name("Public")]
    public class Public : ModuleBase<SocketCommandContext>
    {
        private readonly Random rand;

        public Public(Random rand)
        {
            this.rand = rand;
        }

        [Command("flip")]
        [Summary("Flips a coin, and provides the result in chat.")]
        public async Task Flip()
        {
            await ReplyAsync($"Coin flipped! Result is {Coin()}.");
        }

        private string Coin()
        {
            if ((rand.Next() % 2) == 0)
            {
                return "HEADS";
            }
            else
            {
                return "TAILS";
            }
        }
    }
}
