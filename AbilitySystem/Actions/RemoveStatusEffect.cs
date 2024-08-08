using System;
using System.Collections;
using SoulStitcher.Scripts.Game.AbilitySystem.Actions.Core;
using SoulStitcher.Scripts.Game.AbilitySystem.StatusEffects;
using SoulStitcher.Scripts.Game.Units;
using UnityEngine;
using Zenject;

namespace SoulStitcher.Scripts.Game.AbilitySystem.Actions
{
    [Serializable]
    public class RemoveStatusEffect : AbilityAction
    {
        [SerializeField] private AbilityActionTargetType _target;
        [SerializeField] private StatusEffectType[] _effectTypes;

        private IStatusEffectsManager _effectsManager;

        public override void Initialize(DiContainer container)
        {
            _effectsManager = container.Resolve<IStatusEffectsManager>();
        }

        public override IEnumerator Execute(AbilityContext context)
        {
            if (_target == AbilityActionTargetType.Caster)
                RemoveEffect(context.Caster.Unit);
            else
                foreach (IUnit unit in context.Targets)
                    RemoveEffect(unit);
            
            yield break;
        }

        private void RemoveEffect(IEffectable unit)
        {
            foreach (StatusEffectType effectType in _effectTypes)
                _effectsManager.RemoveStatusEffect(unit, effectType);
        }
    }
}