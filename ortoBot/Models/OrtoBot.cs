using Nanoleaf.Client.Interfaces;
using ortoBot.Effects;
using ortoBot.Helpers;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitchLib.PubSub;
using TwitchLib.PubSub.Events;

namespace ortoBot
{
    public class OrtoBot
    {
        private SimpleTwitchClient twitchClient;
        private INanoleafClient nanoleaf;
        //private DiscordSocketClient discordClient;
        private EffectQueue queue;
        private Settings settings;
        private TwitchPubSub twitchPubSub;

        internal async Task Start()
        {
            var info = await nanoleaf.GetInfoAsync();
            Log.Debug(info.Name);

            Log.Debug("Nanoleaf Effects:");
            var nanoleafEffects = await nanoleaf.GetEffectsAsync();
            nanoleafEffects.ForEach(e => Log.Debug($"\t{e}"));

            if (nanoleafEffects.Count == 0) Log.Warning("No Nanoleaf effects. Create effects in the Nanoleaf app.");

            Log.Debug("Available Effects:");
            var availableEffects = settings.keywords.Where(k => nanoleafEffects.Contains(k.Value)).ToList();
            availableEffects.ForEach(e => Log.Debug($"\t{e}"));

            if (availableEffects.Count() == 0)
            {
                Log.Warning("No available effects. Check your appsettings keywords.");
            } else if (nanoleafEffects.Count > availableEffects.Count)
            {
                Log.Information("Your Nanoleaf has effects, that has not yet been added to the appsettings keywords.");
                nanoleafEffects.Except(availableEffects.Select(kw => kw.Value)).ToList().ForEach(e => Log.Debug($"\t{e}"));
            }

            await nanoleaf.SetEffectAsync(settings.defaultEffect);
        }               

        public OrtoBot(Settings settings)
        {
            this.settings = settings;
            queue = new EffectQueue(settings);
        }

        internal async Task ProcessQueueAsync()
        {
            Console.Write("\u2588");

            if (queue.Count == 0) return;

            Effect br = queue.First.Value;
            switch (br.EffectType)
            {
                case EffectType.BITS:
                    BitEffect be = (BitEffect)br;
                    Log.Information($"BIT EFFECT SET: {be.Username}, {br.EffectName}, {br.Milliseconds / 1000} seconds");
                    if (!be.Continued)
                    {
                        string msgBit = settings.msgEffectSet.Replace("{username}", be.Username)
                            .Replace("{effect}", br.EffectName).Replace("{seconds}", br.Milliseconds / 1000 + "");
                        twitchClient.SendMessage(settings.botJoinChannel, msgBit);
                    }                    
                    await ProcessEffect(br);
                    break;
                case EffectType.SUB:
                    SubEffect se = (SubEffect)br;
                    Log.Information($"SUB EFFECT SET: Tier {se.Tier}, {se.Months} months," +
                        $" {br.EffectName}, {br.Milliseconds / 1000} seconds");
                    await ProcessEffect(br);
                    break;
            }          
        }

        private async Task ProcessEffect(Effect br)
        {            
            await nanoleaf.SetEffectAsync(br.EffectName);
            br.Begin();
            while (br.RemainingMilliseconds > 0)
            {
                Console.Write("\u2592");
                await Task.Delay(1000);
            }
            queue.RemoveFirst();
            if(queue.Count == 0)
            {
                await nanoleaf.SetEffectAsync(settings.defaultEffect);
            }            
        }

        internal async Task HandleReceivedBitsAsync(OnBitsReceivedArgs args)
        {
            Console.WriteLine();
            string msg = $"{args.Username} has just sent {args.BitsUsed} bits!\n" +
                $"(Total {args.TotalBitsUsed}, {args.ChatMessage}, {args.Context})";
            Log.Information(msg);
            if (args.ChatMessage.Contains(settings.requestCommand))
            {
                await TryAddBitRequestToEffectQueueAsync(args);
            }
        }

        internal void ShowCurrent()
        {
            if (queue.Count == 0) return;
            Effect effect = queue.First.Value;
            string msg = $"Current effect: {effect.ToString()}";
            twitchClient.SendMessage(settings.botJoinChannel, msg);
        }

        internal void ShowEffects()
        {
            if (settings.keywords.Count == 0) return;
            List<string> effects = settings.keywords.Keys.ToList();
            string msg = $"You can trigger the lights with bit donations (minimum {settings.minimumBitAmount} bits) " +
                $"by including '{settings.requestCommand}' and one of the following effects to the bit message: " +
                $"({String.Join(" - ", effects)}) Fx. Cheer{settings.minimumBitAmount} {settings.requestCommand}rainbow1";
            twitchClient.SendMessage(settings.botJoinChannel, msg);
        }

        internal void ShowQueue()
        {
            if (queue.Count == 0) return;
            string msg = "";
            foreach(Effect e in queue.Where(e => e.EffectType == EffectType.BITS)){
                msg += e.ToString() + " | ";
            }
            twitchClient.SendMessage(settings.botJoinChannel, msg);
        }

        internal async Task TryAddBitRequestToEffectQueueAsync(OnBitsReceivedArgs e)
        {
            if(e.BitsUsed < settings.minimumBitAmount)
            {
                string msgUnderMinimumBitAmount = settings.msgUnderMinimumBitAmount.Replace("{username}", e.Username);
                twitchClient.SendMessage(settings.botJoinChannel, msgUnderMinimumBitAmount);
                Log.Information("Bit donation to small");
            }

            string effect = await GetEffectFromMessageAsync(e.ChatMessage);
            if (effect == null)
            {
                string msgEffectNotFound = settings.msgEffectNotFound.Replace("{username}", e.Username);
                twitchClient.SendMessage(settings.botJoinChannel, msgEffectNotFound);
                Log.Information("Effect not found");
                return;
            }

            BitEffect be = new BitEffect(effect, e.Username, e.BitsUsed);

            queue.EnqueueBitEffect(be);
            if (queue.Count > 1)
            {
                Log.Information($"EFFECT ADDED TO QUEUE: {e.Username}, {e.BitsUsed} bits, {effect}," +
                    $" {be.Milliseconds / 1000} seconds, position {queue.Count}");
                string msgBitQueue = settings.msgAddedToQueue.Replace("{username}", e.Username).Replace("{position}", queue.Count + "");
                twitchClient.SendMessage(settings.botJoinChannel, msgBitQueue);
            }
        }

        private async Task<string> GetEffectFromMessageAsync(string chatMessage)
        {
            string keyword = ChatMessageParser.GetKeywordValue(chatMessage, settings.requestCommand).ToLower();

            if (!settings.keywords.ContainsKey(keyword)) return null;
            string effect = settings.keywords[keyword];
            List<string> effects = await nanoleaf.GetEffectsAsync();
            if (!effects.Contains(effect)) return null;

            return effect;
        }

        internal void HandleSubscription(OnChannelSubscriptionArgs args)
        {
            Console.WriteLine();
            Log.Information($"{args.Subscription.DisplayName}/{args.Subscription.RecipientDisplayName} has just subscribed!" +
                $" ({args.Subscription.Months} months, tier {args.Subscription.SubscriptionPlan})");
            SubEffect se = new SubEffect(args.Subscription.Months != null? (int)args.Subscription.Months : 0, args.Subscription.SubscriptionPlan, args.Subscription.DisplayName);
            queue.EnqueueSubEffect(se);
        }       

        internal void HandleFollowerAsync(OnFollowArgs args)
        {
            Console.WriteLine();
            Log.Debug($"{args.DisplayName} has just followed!");
        }

        internal void Reset()
        {
            queue.First.Value.Milliseconds = 0;
            queue.Clear();
        }

        //internal void HandleException(Exception e)
        //{
        //    if (discordClient == null) return;
        //    var user = discordClient.GetUser("casrou", "3621");
        //    var task = user.SendMessageAsync(e.ToString());
        //    task.Wait(5000);
        //}

        internal void UpdateSettings(Settings settings)
        {
            this.settings = settings;
        }

        internal void SetNanoleaf(INanoleafClient nanoleaf)
        {
            this.nanoleaf = nanoleaf;            
        }

        internal void SetTwitchClient(SimpleTwitchClient twitchClient)
        {
            this.twitchClient = twitchClient;
        }

        internal void SetTwitchPubSub(SimpleTwitchPubSub twitchPubSub)
        {
            this.twitchPubSub = twitchPubSub;
        }

        //internal void SetDiscordClient(DiscordSocketClient discordClient)
        //{
        //    this.discordClient = discordClient;
        //}
    }
}