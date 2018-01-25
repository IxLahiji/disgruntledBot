using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.IO;
using System.Threading.Tasks;

namespace GrepAdminBot.Services
{
    class Logger
    {
        private readonly DiscordSocketClient discord;
        private readonly CommandService commands;

        private string _logDirectory { get; }
        private string _logFile => Path.Combine(_logDirectory, $"{DateTime.UtcNow.ToString("yyyy-MM-dd")}.txt");

        // DiscordSocketClient and CommandService are injected automatically from the IServiceProvider
        public Logger(DiscordSocketClient discord, CommandService commands)
        {
            _logDirectory = Path.Combine(AppContext.BaseDirectory, "logs");

            this.discord = discord;
            this.commands = commands;

            this.discord.Log += OnLogAsync;
            this.commands.Log += OnLogAsync;
        }

        private Task OnLogAsync(LogMessage msg)
        {
            if (!Directory.Exists(_logDirectory))     // Create the log directory if it doesn't exist
                Directory.CreateDirectory(_logDirectory);
            if (!File.Exists(_logFile))               // Create today's log file if it doesn't exist
                File.Create(_logFile).Dispose();

            string logText = $"{DateTime.UtcNow.ToString("hh:mm:ss")} [{msg.Severity}] {msg.Source}: {msg.Exception?.ToString() ?? msg.Message}";
            File.AppendAllText(_logFile, logText + "\n");     // Write the log text to a file

            return Console.Out.WriteLineAsync(logText);       // Write the log text to the console
        }
    }
}
