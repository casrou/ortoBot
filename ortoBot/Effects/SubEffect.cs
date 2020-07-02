using ortoBot.Effects;
using System;
using TwitchLib.PubSub.Enums;

namespace ortoBot
{
    public class SubEffect : Effect
    {
        public int Months { get; set; }
        public SubscriptionPlan Tier { get; set; }

        public SubEffect(int months, SubscriptionPlan tier, string username) : base(EffectType.SUB, username)
        {
            Months = months;
            Tier = tier;
        }
    }
}