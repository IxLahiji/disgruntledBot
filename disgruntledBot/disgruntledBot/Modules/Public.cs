using Discord;
using Discord.Commands;

using System;
using System.Threading.Tasks;

using GiphyDotNet.Manager;
using GiphyDotNet.Model.Parameters;

using Microsoft.Extensions.Configuration;

namespace disgruntledBot.Modules
{
    [Name("Public")]
    [Summary("Commands that can be invoked anywhere, by anyone.")]
    public class Public : ModuleBase<SocketCommandContext>
    {
        private readonly Random rand;
        private readonly IConfigurationRoot config;

        public Public(Random rand, IConfigurationRoot config)
        {
            this.rand = rand;
            this.config = config;
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
            // Check to make sure number provided is greater than 0.
            if (numFlips > 0)
            {
                await ReplyAsync($"{numFlips} coins flipped! Results are {MultCoins(numFlips)}.");
            }
            else
            {
                await ReplyAsync("Please use a number greater than 0.");
            }
        }

        private string MultCoins(int n)
        {
            // Put first coin result, this is allowed because n > 0.
            string results = Coin();

            // Loop for rest of tosses starts at 1, to account for initial toss.
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

        [Command("gif"), Priority(0)]
        [Summary("Posts a random gif.")]
        public async Task Gif()
        {
            // Get the giphy token from the config file.
            string giphyToken = this.config["tokens:giphy"];

            if (string.IsNullOrWhiteSpace(giphyToken))
            {
                await ReplyAsync("No Giphy app token provided. Please enter token into the `config.json` file found in the applications root directory.");
            }
            else
            {
                Giphy giphy = new Giphy(giphyToken);

                RandomParameter randomGif = new RandomParameter();

                // Returns gif results.
                var gifResult = await giphy.RandomGif(randomGif);

                var imageUrl = new EmbedBuilder()
                    .WithImageUrl(gifResult.Data.ImageOriginalUrl).Build();

                await ReplyAsync("", embed: imageUrl);
            }
        }

        [Command("gif"), Priority(1)]
        [Summary("Posts a gif related to the description provided.")]
        public async Task Gif([Remainder]string query)
        {
            // Get the giphy token from the config file.
            string giphyToken = this.config["tokens:giphy"];

            if (string.IsNullOrWhiteSpace(giphyToken))
            {
                await ReplyAsync("No Giphy app token provided. Please enter token into the `config.json` file found in the applications root directory.");
            }
            else
            {
                Giphy giphy = new Giphy(giphyToken);

                SearchParameter searchParameter = new SearchParameter()
                {
                    Query = query
                };

                // Returns gif results.
                var gifResult = await giphy.GifSearch(searchParameter);

                if (gifResult.Data.Length > 0)
                {
                    var imageUrl = new EmbedBuilder()
                    .WithImageUrl(gifResult.Data[rand.Next() % gifResult.Data.Length].Images.Original.Url).Build();

                    await ReplyAsync("", embed: imageUrl);
                }
                else
                {
                    await ReplyAsync("[Error]: Tag provided returned no results!");
                }
            }
        }

        [Command("roll"), Priority(0)]
        [Summary("Rolls a die and provides result in chat.")]
        public async Task Roll()
        {
            await ReplyAsync($"Die rolled! Result is {Dice()}.");
        }

        [Command("roll"), Priority(1)]
        [Summary("Rolls multiple dice, based on a provided positive number, and provides results in chat.")]
        public async Task Roll([Remainder]int numRolls)
        {
            // Check to make sure number provided is greater than 0.
            if (numRolls > 0)
            {
                await ReplyAsync($"{numRolls} dice rolled! Results are {MultDice(numRolls)}.");
            }
            else
            {
                await ReplyAsync("Please use a number greater than 0.");
            }
        }

        private string Dice()
        {
            return $"{rand.Next() % 6 + 1}";
        }

        private string MultDice(int n)
        {
            string results = Dice();

            for (int i = 1; i < n; i++) results += $", {Dice()}";
            return results;
        }
    }
}
