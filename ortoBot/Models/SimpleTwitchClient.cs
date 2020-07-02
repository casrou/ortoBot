using ortoBot.Helpers;
using Serilog;
using System;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.PubSub.Events;

namespace ortoBot
{
    public class SimpleTwitchClient : TwitchClient
    {
        private Settings settings;
        private readonly OrtoBot ortoBot;

        public SimpleTwitchClient(Settings settings, OrtoBot ortoBot)
        {
            this.ortoBot = ortoBot;
            this.settings = settings;

            ConnectionCredentials credentials = new ConnectionCredentials(settings.botUsername, settings.botOauth);
            Initialize(credentials, settings.botJoinChannel);

            AddChatCommandIdentifier('!');
            OnChatCommandReceived += ProcessReceivedChatCommand;
            //OnBeingHosted += ProcessIncomingHost; // Hosting events only work if broadcaster
            OnRaidNotification += ProcessIncomingRaid;

            Connect();
            Log.Information($"Twitch client connected!");
        }

        private void ProcessIncomingRaid(object sender, OnRaidNotificationArgs e)
        {
            RaidNotification raid = e.RaidNotification;
            string msg = $"{raid.DisplayName}, {raid.MsgParamDisplayName}, {raid.MsgParamViewerCount}";
            Log.Information(msg);
        }

        private void ProcessIncomingHost(object sender, OnBeingHostedArgs e)
        {
            BeingHostedNotification host = e.BeingHostedNotification;
            string msg = $"{host.HostedByChannel}, auto: {host.IsAutoHosted}, {host.Viewers} viewers";
            Log.Information(msg);
        }

        public virtual new void SendMessage(string channel, string message, bool dryRun = false)
        {
            base.SendMessage(channel, message);
        }

        private async void ProcessReceivedChatCommand(object sender, OnChatCommandReceivedArgs e)
        {
            if (!e.Command.ChatMessage.IsModerator && !e.Command.ChatMessage.IsBroadcaster) return;
            if (!e.Command.CommandText.ToLower().Equals("ortobot")) return;

            string message = e.Command.ChatMessage.Message.ToLower();

            if (message.Contains("reset"))
            {
                ortoBot.Reset();
            }
            else if (message.Contains("queue"))
            {
                ortoBot.ShowQueue();
            }            
            else if (message.Contains("current"))
            {
                ortoBot.ShowCurrent();
            }
            else if (message.Contains("effects"))
            {
                ortoBot.ShowEffects();
            }
            else if (message.Contains(settings.requestCommand))
            {
                int bits;
                int.TryParse(ChatMessageParser.GetKeywordValue(message, "bits:"), out bits);
                if (bits <= 0) return;
                
                string username = ChatMessageParser.GetKeywordValue(message, "username:");
                if (string.IsNullOrEmpty(username)) return;

                OnBitsReceivedArgs obr = new OnBitsReceivedArgs();
                obr.Username = username;
                obr.BitsUsed = bits;
                obr.ChatMessage = e.Command.ChatMessage.Message;
                Console.WriteLine();
                string msg = $"{e.Command.ChatMessage.DisplayName} has just added an effect with the mod tool: {e.Command.ChatMessage.Message}";
                Log.Information(msg);
                await ortoBot.TryAddBitRequestToEffectQueueAsync(obr);
            }        
        }
    }
}