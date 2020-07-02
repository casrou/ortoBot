using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TwitchLib.PubSub.Enums;

namespace ortoBot.Effects
{
    public class EffectQueue : LinkedList<Effect>
    {
        private Settings settings;
        //private LinkedList<Effect> queue;

        public bool subInProgress { get;  set; }
        private BitEffect bitInProgress { get; set; }

        public EffectQueue(Settings settings)
        {
            this.settings = settings;
        }

        /*public EffectQueue(Settings settings)
        {
            this.settings = settings;
            //queue = new LinkedList<Effect>();
        }*/

        public void EnqueueSubEffect(SubEffect se)
        {
            se.Milliseconds = settings.subEffectSec * 1000;

            int months = se.Months;
            string subEffect = settings.subEffect1;
            if (months < 3)
            {
                subEffect = settings.subEffect1;
            }
            else if (months >= 3 && months < 6)
            {
                subEffect = settings.subEffect3;
            }
            else if (months >= 6)
            {
                subEffect = settings.subEffect6;
            }
            se.EffectName = subEffect;

            Effect firstSubEffect = this.FirstOrDefault(e => e.EffectType == EffectType.SUB);
            if (firstSubEffect != null)
            {
                // A sub effect is already in the queue
                se.Milliseconds = 1000;
                //firstSubEffect.Milliseconds += 1000;
            }

            BitEffect firstBitEffect = (BitEffect) this.FirstOrDefault(e => e.EffectType == EffectType.BITS);            
            if (firstBitEffect != null)
            {
                // A bit effect is already in the queue
                LinkedListNode<Effect> seNode = AddBefore(Find(firstBitEffect), se);
                if (firstBitEffect.Running)
                {
                    //Effect lastSubEffect = this.LastOrDefault(e => e.EffectType == EffectType.SUB);
                    firstBitEffect.Milliseconds = firstBitEffect.RemainingMilliseconds;
                    firstBitEffect.Running = false;
                    firstBitEffect.Continued = true;
                    Remove(firstBitEffect);
                    AddAfter(seNode, firstBitEffect);
                }
            } else
            {
                AddLast(se);
            }
        }

        public void EnqueueBitEffect(BitEffect bitEffect)
        {
            bitEffect.Milliseconds = (int)(bitEffect.BitsUsed * settings.bitsToSecRatio * 1000);

            AddLast(bitEffect);
        }
    }
}
