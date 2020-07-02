using ortoBot.Effects;
using System;
using System.Collections.Generic;
using System.Text;
using TwitchLib.PubSub.Events;

namespace ortoBot
{
    public class BitEffect : Effect, ICloneable
    {
        public int BitsUsed { get; internal set; }
        public bool Continued { get; set; }

        public BitEffect(string effectName, string username, int bitsUsed) : base(EffectType.BITS, username)
        {
            EffectName = effectName;
            BitsUsed = bitsUsed;
            Continued = false;
        }      

        public object Clone()
        {
            return new BitEffect(EffectName, Username, BitsUsed);
        }
    }
}
