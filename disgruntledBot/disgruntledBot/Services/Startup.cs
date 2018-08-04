using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System;
using System.Reflection;
using System.Threading.Tasks;


namespace disgruntledBot.Services
{
    class Startup
    {
        private readonly DiscordSocketClient discord;
        private readonly CommandService commands;
        private readonly IConfigurationRoot config;

        // DiscordSocketClient, CommandService, and IConfigurationRoot are injected automatically from the IServiceProvider
        public Startup(
            DiscordSocketClient discord,
            CommandService commands,
            IConfigurationRoot config)
        {
            this.config = config;
            this.discord = discord;
            this.commands = commands;
        }

        public async Task StartAsync()
        {
            string discordToken = this.config["tokens:discord"];     // Get the discord token from the config file
            if (string.IsNullOrWhiteSpace(discordToken))
                throw new Exception("Please enter your bot's token into the `_config.json` file found in the applications root directory.");

            await this.discord.LoginAsync(TokenType.Bot, discordToken);     // Login to discord
            await this.discord.StartAsync();                                // Connect to the websocket

            await this.commands.AddModulesAsync(Assembly.GetEntryAssembly());     // Load commands and modules into the command service
        }
    }
}
