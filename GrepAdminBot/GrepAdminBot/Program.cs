using Discord;
using Discord.Commands;
using Discord.WebSocket;

using GrepAdminBot.Services;
using GrepAdminBot.Model.ConfigTemplates;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace GrepAdminBot
{
    class Program
    {
        public static void Main(string[] args)
            => new Program().StartAsync().GetAwaiter().GetResult();

        private IConfigurationRoot _config;

        private const String CONFIG_FILE = "config.json";

        public async Task StartAsync()
        {
            Console.WriteLine();
            //Generate empty JSON configuration file if one does not exist
            if (!File.Exists(CONFIG_FILE))
            {
                File.Create(CONFIG_FILE).Dispose();

                ConfigurationSettings configSettings = new ConfigurationSettings();

                string jsonOutput = JsonConvert.SerializeObject(configSettings, Formatting.Indented);
                File.WriteAllText(CONFIG_FILE, jsonOutput);
            }

            //Read configuration data
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile(CONFIG_FILE, optional: true, reloadOnChange: true);
            
            _config = builder.Build();

            var services = new ServiceCollection()      // Begin building the service provider
                .AddSingleton(new DiscordSocketClient(new DiscordSocketConfig     // Add the discord client to the service provider
                {
                    LogLevel = LogSeverity.Verbose,
                    MessageCacheSize = 1000             // Tell Discord.Net to cache 1000 messages per channel
                }))
                .AddSingleton(new CommandService(new CommandServiceConfig     // Add the command service to the service provider
                {
                    DefaultRunMode = RunMode.Async,     // Force all commands to run async
                    LogLevel = LogSeverity.Verbose
                }))
                .AddSingleton<Commands>()
                .AddSingleton<Startup>()
                .AddSingleton<Logger>()
                .AddSingleton(_config)
                .AddSingleton<Random>();

            var provider = services.BuildServiceProvider();     // Create the service provider

            provider.GetRequiredService<Logger>();
            await provider.GetRequiredService<Startup>().StartAsync();
            provider.GetRequiredService<Commands>();

            await Task.Delay(-1);     // Prevent the application from closing
        }
    }
}
