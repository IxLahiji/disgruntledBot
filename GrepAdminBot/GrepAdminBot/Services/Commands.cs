using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System;

namespace GrepAdminBot.Services
{
    class Commands
    {
        private readonly DiscordSocketClient discord;
        private readonly CommandService commands;
        private readonly IConfigurationRoot config;
        private readonly IServiceProvider provider;

        // DiscordSocketClient, CommandService, IConfigurationRoot, and IServiceProvider are injected automatically from the IServiceProvider
        public Commands(
            DiscordSocketClient discord,
            CommandService commands,
            IConfigurationRoot config,
            IServiceProvider provider)
        {
            this.discord = discord;
            this.commands = commands;
            this.config = config;
            this.provider = provider;

            discord.MessageReceived += OnMessageReceivedAsync;
        }

        private async Task OnMessageReceivedAsync(SocketMessage s)
        {
            var msg = s as SocketUserMessage;     // Ensure the message is from a user/bot
            if (msg == null) return;
            if (msg.Author.Id == this.discord.CurrentUser.Id) return;     // Ignore self when checking commands

            var context = new SocketCommandContext(this.discord, msg);     // Create the command context

            int argPos = 0;     // Check if the message has a valid command prefix
            if (msg.HasStringPrefix(this.config["prefix"], ref argPos) || msg.HasMentionPrefix(this.discord.CurrentUser, ref argPos))
            {
                var result = await this.commands.ExecuteAsync(context, argPos, this.provider);     // Execute the command

                if (!result.IsSuccess)     // If not successful, reply with the error.
                    await context.Channel.SendMessageAsync(result.ToString());
            }
        }
    }
}

