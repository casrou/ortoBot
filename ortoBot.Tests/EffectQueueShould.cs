using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ortoBot.Effects;
using TwitchLib.PubSub.Enums;
using Xunit;

namespace ortoBot.Tests
{
    public class EffectQueueShould
    {
        private EffectQueue effectQueue;
        private Settings settings;

        public EffectQueueShould()
        {
            settings = new Settings()
            {
                subEffect1 = "subEffect1",
                subEffect3 = "subEffect3",
                subEffect6 = "subEffect6",
                bitsToSecRatio = 0.6,
                subEffectSec = 10
            };
            effectQueue = new EffectQueue(settings);
        }

        [Fact]
        public void AddSubEffectsBeforeBitEffects()
        {
            BitEffect bitEffect = new BitEffect("fish", "test", 100);
            effectQueue.EnqueueBitEffect(bitEffect);
            SubEffect subEffect = new SubEffect(1, SubscriptionPlan.Tier1, "test");
            effectQueue.EnqueueSubEffect(subEffect);
            BitEffect bitEffect2 = new BitEffect("fish", "test2", 100);
            effectQueue.EnqueueBitEffect(bitEffect2);

            Assert.Equal(effectQueue.First.Value, subEffect);
        }

        [Fact]
        public void AddBitEffectsAfterSubEffects()
        {
            SubEffect subEffect = new SubEffect(1, SubscriptionPlan.Tier1, "test");
            effectQueue.EnqueueSubEffect(subEffect);

            BitEffect bitEffect = new BitEffect("fish", "test", 100);
            effectQueue.EnqueueBitEffect(bitEffect);

            SubEffect subEffect2 = new SubEffect(2, SubscriptionPlan.Tier1, "test2");
            effectQueue.EnqueueSubEffect(subEffect2);

            Assert.Equal(effectQueue.Last.Value, bitEffect);
        }

        [Fact]
        public void AddTwoBitEffectsInCorrectOrder()
        {
            BitEffect bitEffect = new BitEffect("fish", "test", 100);
            effectQueue.EnqueueBitEffect(bitEffect);

            BitEffect bitEffect2 = new BitEffect("fish", "test", 100);
            effectQueue.EnqueueBitEffect(bitEffect2);

            LinkedListNode<Effect> first = effectQueue.First;
            Assert.Equal(first.Value, bitEffect);
            LinkedListNode<Effect> second = first.Next;
            Assert.Equal(second.Value, bitEffect2);
        }

        [Fact]
        public void AddTwoSubEffectsInCorrectOrder()
        {
            SubEffect subEffect = new SubEffect(1, SubscriptionPlan.Tier1, "test");
            effectQueue.EnqueueSubEffect(subEffect);

            SubEffect subEffect2 = new SubEffect(2, SubscriptionPlan.Tier1, "test2");
            effectQueue.EnqueueSubEffect(subEffect2);

            LinkedListNode<Effect> first = effectQueue.First;
            Assert.Equal(first.Value, subEffect);
            LinkedListNode<Effect> second = first.Next;
            Assert.Equal(second.Value, subEffect2);
        }

        [Fact]
        public void DecreaseDurationForConsecutiveSubEffects()
        {
            SubEffect subEffect = new SubEffect(1, SubscriptionPlan.Tier1, "test");
            effectQueue.EnqueueSubEffect(subEffect);

            SubEffect subEffect2 = new SubEffect(2, SubscriptionPlan.Tier1, "test2");
            effectQueue.EnqueueSubEffect(subEffect2);

            SubEffect subEffect3 = new SubEffect(2, SubscriptionPlan.Tier1, "test2");
            effectQueue.EnqueueSubEffect(subEffect3);

            LinkedListNode<Effect> first = effectQueue.First;
            Assert.Equal(10000, first.Value.Milliseconds);
            LinkedListNode<Effect> second = first.Next;
            Assert.Equal(1000, second.Value.Milliseconds);
            LinkedListNode<Effect> third = second.Next;
            Assert.Equal(1000, third.Value.Milliseconds);
        }

        [Fact]
        public void ChooseSubEffectFromNumberOfMonths()
        {
            SubEffect subEffect = new SubEffect(1, SubscriptionPlan.Tier1, "test");
            effectQueue.EnqueueSubEffect(subEffect);

            SubEffect subEffect2 = new SubEffect(4, SubscriptionPlan.Tier1, "test2");
            effectQueue.EnqueueSubEffect(subEffect2);

            SubEffect subEffect3 = new SubEffect(7, SubscriptionPlan.Tier1, "test2");
            effectQueue.EnqueueSubEffect(subEffect3);

            LinkedListNode<Effect> first = effectQueue.First;
            Assert.Equal(settings.subEffect1, first.Value.EffectName);
            LinkedListNode<Effect> second = first.Next;
            Assert.Equal(settings.subEffect3, second.Value.EffectName);
            LinkedListNode<Effect> third = second.Next;
            Assert.Equal(settings.subEffect6, third.Value.EffectName);
        }

        [Fact]
        public void SetSubEffectDurationFromSettings()
        {
            SubEffect subEffect = new SubEffect(1, SubscriptionPlan.Tier1, "test");
            effectQueue.EnqueueSubEffect(subEffect);

            LinkedListNode<Effect> first = effectQueue.First;
            Assert.Equal(settings.subEffectSec * 1000, first.Value.Milliseconds);
        }

        [Fact]
        public void UpdateBitEffectDurationOnAdd()
        {
            int bits = 100;
            BitEffect bitEffect = new BitEffect("fish", "test", bits) { };
            Assert.Equal(0, bitEffect.Milliseconds);
            effectQueue.EnqueueBitEffect(bitEffect);
            Assert.Equal(bits * settings.bitsToSecRatio * 1000, bitEffect.Milliseconds);
        }

        [Fact]
        public void HandleSubEffectWithRunningBitEffect()
        {
            BitEffect bitEffect = new BitEffect("fish", "test", 100);
            effectQueue.EnqueueBitEffect(bitEffect);

            int duration = bitEffect.Milliseconds; // 60000

            bitEffect.Begin();

            Assert.True(bitEffect.Running);

            SubEffect subEffect = new SubEffect(1, SubscriptionPlan.Tier1, "test");
            effectQueue.EnqueueSubEffect(subEffect);            
            
            LinkedListNode<Effect> sub = effectQueue.First;
            Assert.Equal(sub.Value, subEffect);

            Assert.Equal(sub.Next.Value, bitEffect);
            Assert.InRange(bitEffect.Milliseconds, bitEffect.RemainingMilliseconds, duration + sub.Value.Milliseconds);
        }

        [Fact]
        public void SetBitEffectAsContinuedIfResumedAfterSubEffect()
        {
            BitEffect bitEffect = new BitEffect("fish", "test", 100);
            bitEffect.Running = true;
            effectQueue.EnqueueBitEffect(bitEffect);
            
            Assert.False(bitEffect.Continued);

            SubEffect subEffect = new SubEffect(2, SubscriptionPlan.Tier1, "test");
            effectQueue.EnqueueSubEffect(subEffect);

            BitEffect queueEffect = (BitEffect)effectQueue.First(e => e.EffectType == EffectType.BITS);
            Assert.True(queueEffect.Continued);
        }
    }
}
