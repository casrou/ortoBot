using Serilog;
using System;
using System.Threading.Tasks;
using TwitchLib.PubSub;
using TwitchLib.PubSub.Events;

namespace ortoBot
{
    class SimpleTwitchPubSub : TwitchPubSub
    {
        public SimpleTwitchPubSub(Settings settings, OrtoBot ortoBot)
        {
            OnPubSubServiceConnected += (sender, e) => SendTopics(settings.oauth);
            OnChannelSubscription += (object sender, OnChannelSubscriptionArgs args) => ortoBot.HandleSubscription(args);
            OnFollow += (object sender, OnFollowArgs args) => ortoBot.HandleFollowerAsync(args);
            OnBitsReceived += async (object sender, OnBitsReceivedArgs args) => await ortoBot.HandleReceivedBitsAsync(args);
            
            OnListenResponse += onListenResponse;
            /*OnStreamUp += onStreamUp;
            OnStreamDown += onStreamDown;*/

            ListenToFollows(settings.channelId);
            ListenToBitsEvents(settings.channelId);
            //ListenToVideoPlayback(settings.channelName);
            ListenToSubscriptions(settings.channelId);

            Connect();
            Log.Information($"Twitch PubSub connected!");
        }

        private static void onFollow(object sender, OnFollowArgs e)
        {
            string msg = $"{e.DisplayName} has just followed!";
            Log.Information("\n" + msg);
        }

        private static void onListenResponse(object sender, OnListenResponseArgs e)
        {
            if (!e.Successful)
                throw new Exception($"Failed to listen! Response: {e.Response}");
        }

        /*private static void onStreamUp(object sender, OnStreamUpArgs e)
        {
            Log.Information($"Stream just went up! Play delay: {e.PlayDelay}, server time: {e.ServerTime}");
        }

        private static void onStreamDown(object sender, OnStreamDownArgs e)
        {
            Log.Information($"Stream just went down! Server time: {e.ServerTime}");
        }*/
    }
}
