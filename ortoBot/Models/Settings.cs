using System;
using System.Collections.Generic;
using System.Text;

namespace ortoBot
{
    public class Settings
    {       
        // Twitch (PubSub)
        public string channelId { get; set; }
        public string oauth { get; set; }
        // Nanoleaf
        public string ip { get; set; }
        public string authToken { get; set; }
        // Twitch (Client)
        public string botUsername { get; set; }
        public string botOauth { get; set; }
        public string botJoinChannel { get; set; }
        // Discord
        //public string discord_token { get; set; }
        // Custom
        public Dictionary<string, string> keywords { get; set; }
        public string subEffect1 { get; set; }
        public string subEffect3 { get; set; }
        public string subEffect6 { get; set; }
        public int subEffectSec { get; set; }
        public double bitsToSecRatio { get; set; }
        public int minimumBitAmount { get; set; }
        public string defaultEffect { get; set; }
        public bool debug { get; set; }
        public string requestCommand { get; set; }
        public string msgEffectSet { get; set; }
        public string msgAddedToQueue { get; set; }
        public string msgEffectNotFound { get; set; }
        public string msgUnderMinimumBitAmount { get; set; }
    }
}