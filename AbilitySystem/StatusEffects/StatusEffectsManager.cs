using System.Collections.Generic;
using JetBrains.Annotations;
using SoulStitcher.Scripts.Game.Units;
using UnityEngine;

namespace SoulStitcher.Scripts.Game.AbilitySystem.StatusEffects
{
    [UsedImplicitly]
    public class StatusEffectsManager : IStatusEffectsManager
    {
        private readonly Dictionary<IEffectable, Dictionary<StatusEffectType, StatusEffect>> _runningEffects = new();
        private readonly IStatusEffectFactory _factory;

        public StatusEffectsManager(IStatusEffectFactory factory)
        {
            _factory = factory;
        }
        
        public void ApplyStatusEffect(IEffectable unit, StatusEffect statusEffect)
        {
            if (unit.HasEffect(statusEffect.StatusEffectType))
            {
                Debug.Log("unit has effect");
                if (statusEffect.DurationType == StatusEffectDurationType.Time)
                {
                    StatusEffect attachedEffect = _runningEffects[unit][statusEffect.StatusEffectType];
                    unit.ResetEffect(attachedEffect);
                    Debug.Log("reset effect");
                }

                return;
            }

            StatusEffect effect = _factory.GetEffect(statusEffect);
            unit.ApplyEffect(effect);
            effect.Finalized += RemoveStatusEffect;
            effect.Apply(unit);

            if (_runningEffects.TryGetValue(unit, out Dictionary<StatusEffectType, StatusEffect> runningEffect))
                runningEffect.Add(effect.StatusEffectType, effect);
            else
                _runningEffects.Add(unit,  new Dictionary<StatusEffectType, StatusEffect> { { effect.StatusEffectType, effect } });
        }

        public void RemoveStatusEffect(IEffectable unit, StatusEffectType type)
        {
            if (!unit.HasEffect(type))
                return;
            
            unit.RemoveEffect(type);
            _runningEffects[unit][type].Finalized -= RemoveStatusEffect;
            _runningEffects[unit][type].CleanUp();
            _runningEffects[unit].Remove(type);

            if (_runningEffects[unit].Count == 0)
                _runningEffects.Remove(unit);
        }
    }
}