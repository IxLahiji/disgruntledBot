using Discord;
using Discord.Commands;
using Discord.WebSocket;

using System;
using System.Threading.Tasks;

using GiphyDotNet.Manager;
using GiphyDotNet.Model.Parameters;

namespace GrepAdminBot.Modules
{
    [Name("Public")]
    [Summary("Commands that can be invoked anywhere, by anyone.")]
    public class Public : ModuleBase<SocketCommandContext>
    {
        private readonly Random rand;

        public Public(Random rand)
        {
            this.rand = rand;
        }

        [Command("flip"), Priority(0)]
        [Summary("Flips a coin, and provides the result in chat.")]
        public async Task Flip()
        {
            await ReplyAsync($"Coin flipped! Result is {Coin()}.");
        }

        [Command("flip"), Priority(1)]
        [Summary("Flips multiple coins, based on a provided positive number, and provides the results in chat.")]
        public async Task Flip([Remainder]int numFlips)
        {
            // Check to make sure number provided is greater than 0
            if (numFlips > 0)
            {
                await ReplyAsync($"{numFlips} coins flipped! Results is {MultCoins(numFlips)}.");
            }
            else
            {
                await ReplyAsync("Please use a number greater than 0.");
            }
        }

        private string MultCoins(int n)
        {
            // Put first coin result, this is allowed because n > 0
            string results = Coin();
            // Loop for rest of tosses starts at 1, to account for initial toss
            for (int i = 1; i < n; i++) results += (", " + Coin());
            return results;
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

        [Command("gif")]
        [Summary("Posts a gif realted to the description provided.")]
        public async Task Gif([Remainder]string query)
        {
            Giphy giphy = new Giphy("");

            SearchParameter searchParameter = new SearchParameter()
            {
                Query = query
            };

            // Returns gif results
            var gifResult = await giphy.GifSearch(searchParameter);

            var imageUrl = new EmbedBuilder()
                .WithImageUrl(gifResult.Data[0].Url).Build();

            await ReplyAsync("", embed: imageUrl);
        }
    }
}
