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

            // Generate empty JSON configuration file if one does not exist.
            if (!File.Exists(CONFIG_FILE))
            {
                File.Create(CONFIG_FILE).Dispose();

                ConfigurationSettings configSettings = new ConfigurationSettings();

                string jsonOutput = JsonConvert.SerializeObject(configSettings, Formatting.Indented);
                File.WriteAllText(CONFIG_FILE, jsonOutput);
            }

            // Read configuration data.
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile(CONFIG_FILE, optional: true, reloadOnChange: true);
            
            _config = builder.Build();

            // Begin building the service provider with the following settings.
            // Cache 1000 messages per channel.
            // Force all commands to run async.
            // Verbose logging for both Discord client and Command Service.
            var services = new ServiceCollection()
                .AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
                {
                    LogLevel = LogSeverity.Verbose,
                    MessageCacheSize = 1000
                }))
                .AddSingleton(new CommandService(new CommandServiceConfig
                {
                    DefaultRunMode = RunMode.Async,
                    LogLevel = LogSeverity.Verbose
                }))
                .AddSingleton<Commands>()
                .AddSingleton<Startup>()
                .AddSingleton<Logger>()
                .AddSingleton(_config)
                .AddSingleton<Random>();

            var provider = services.BuildServiceProvider();

            provider.GetRequiredService<Logger>();
            await provider.GetRequiredService<Startup>().StartAsync();
            provider.GetRequiredService<Commands>();

            // Prevent the application from closing.
            await Task.Delay(-1);
        }
    }
}
