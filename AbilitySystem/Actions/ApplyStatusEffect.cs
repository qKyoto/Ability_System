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
    public class ApplyStatusEffect : AbilityAction
    {
        [SerializeField] private AbilityActionTargetType _target;
        [SerializeField] private StatusEffect[] _statusEffects;

        private IStatusEffectsManager _effectsManager;

        public override void Initialize(DiContainer container)
        {
            _effectsManager = container.Resolve<IStatusEffectsManager>();
        }

        public override IEnumerator Execute(AbilityContext context)
        {
            if (_target == AbilityActionTargetType.Caster)
                Apply(context.Caster.Unit);
            else
                foreach (IUnit unit in context.Targets)
                    Apply(unit);
            
            yield break;
        }

        private void Apply(IEffectable unit)
        {
            foreach (StatusEffect statusEffect in _statusEffects)
                _effectsManager.ApplyStatusEffect(unit, statusEffect);
        }
    }
}