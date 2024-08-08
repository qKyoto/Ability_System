using System;
using UnityEngine;

namespace SoulStitcher.Scripts.Game.AutoAttackSystem.Core
{
    public class AttackEventReceiver : MonoBehaviour
    {
        public event Action<bool> DamageCapabilityChanged;
        public event Action DamageReceived;
        public event Action AnimationStarted;
        public event Action AnimationEnded;

        public void OnAnimationStarted(AnimationEvent animationEvent) =>
            AnimationStarted?.Invoke();
        
        public void OnAnimationEnded(AnimationEvent animationEvent) =>
            AnimationEnded?.Invoke();
        
        public void OnDealDamageStarted(AnimationEvent animationEvent) =>
            DamageCapabilityChanged?.Invoke(true);

        public void OnDealDamageEnded(AnimationEvent animationEvent) =>
            DamageCapabilityChanged?.Invoke(false);

        public void OnDamageReceived() =>
            DamageReceived?.Invoke();
    }
}