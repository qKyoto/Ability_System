using System;
using System.Collections;
using SoulStitcher.Scripts.Game.AbilitySystem.Actions.Core;
using SoulStitcher.Scripts.Game.AbilitySystem.StatusEffects;
using SoulStitcher.Scripts.Game.StatsSystem;
using SoulStitcher.Scripts.Game.Units;
using UnityEngine;

namespace SoulStitcher.Scripts.Game.AbilitySystem.Actions
{
    [Serializable]
    public class ChangeAttribute : AbilityAction
    {
        [SerializeField] private AbilityActionTargetType _target;
        [SerializeField] private AttributeType _type;
        [SerializeField] private float _value;

        public override IEnumerator Execute(AbilityContext context)
        {
            if (_target == AbilityActionTargetType.Caster)
                Change(context.Caster.Unit);
            else
                foreach (IUnit unit in context.Targets)
                    Change(unit);
            
            yield break;
        }

        private void Change(IEffectable modifiable)
        {
            modifiable.StatsSheet.GetAttribute(_type).Value += _value;
        }
    }
}