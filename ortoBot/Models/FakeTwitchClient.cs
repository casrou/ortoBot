using Serilog;
using TwitchLib.Client;

namespace ortoBot
{
    internal class FakeTwitchClient : SimpleTwitchClient
    {
        public FakeTwitchClient(Settings settings, OrtoBot ortoBot) : base(settings, ortoBot) { }

        public override void SendMessage(string channel, string message, bool dryRun = false)
        {
            Log.Information("DEBUG: " + message);
        }
    }
}