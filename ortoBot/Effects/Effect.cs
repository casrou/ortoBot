using System;
using System.Collections.Generic;
using System.Text;

namespace ortoBot.Effects
{
    public class Effect
    {
        /*public Effect(EffectType effectType, string effectName, DateTime timestamp)
        {
            EffectType = effectType;
            EffectName = effectName;
            Timestamp = timestamp;
        }*/

        public Effect(EffectType effectType, string username)
        {
            EffectType = effectType;
            Username = username;
            //EffectName = effectName;
        }

        /*public Effect(EffectType effectType)
        {
            EffectType = effectType;
        }*/

        public EffectType EffectType { get; set; }
        public string EffectName { get; set; }
        public int Milliseconds { get; set; }
        public string Username { get; internal set; }
        public bool Running { get; set; }

        public DateTime Timestamp { get; set; }

        public void Begin()
        {
            Timestamp = DateTime.Now;
            Running = true;
        }

        public int RemainingMilliseconds
        {
            get
            {
                DateTime now = DateTime.Now;             
                return (int) Math.Max(0, Milliseconds - now.Subtract(Timestamp).TotalMilliseconds);
            }
        }

        public override string ToString()
        {
            string msg = $"@{Username}: {EffectName} ({RemainingMilliseconds / 1000} seconds)";
            if (Running)
            {
                msg = msg + "*";
            }
            return msg;
        }
    }

    public enum EffectType
    {
        BITS, SUB
    }
}
