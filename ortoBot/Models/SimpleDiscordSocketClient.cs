using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ortoBot
{
    class SimpleDiscordSocketClient : DiscordSocketClient {
        private Settings settings;

        public SimpleDiscordSocketClient(Settings settings)
        {
            this.settings = settings;
            //Log += LogDiscord;            
        }

        public async Task Initialize(string discord_token)
        {
            await LoginAsync(TokenType.Bot, discord_token);
            await StartAsync();
            Serilog.Log.Information($"Discord client connected!");
        }

        private static Task LogDiscord(LogMessage msg)
        {
            Serilog.Log.Information(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
