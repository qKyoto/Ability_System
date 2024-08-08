using System;
using System.Collections;
using SoulStitcher.Scripts.Game.AbilitySystem.Actions.Core;
using SoulStitcher.Scripts.Game.Units;
using UnityEngine;

namespace SoulStitcher.Scripts.Game.AbilitySystem.Actions
{
    [Serializable]
    public class OverrideAnimation : AbilityAction
    {
        [SerializeField] private AbilityActionTargetType _target;
        [SerializeField] private string _nameAnimation;

        public override IEnumerator Execute(AbilityContext context)
        {
            if (_target == AbilityActionTargetType.Caster)
                context.Caster.Unit.AnimationsControllerBase.PlayAnimation(Animator.StringToHash(_nameAnimation));
            else
                foreach (IUnit unit in context.Targets)
                    unit.AnimationsControllerBase.PlayAnimation(Animator.StringToHash(_nameAnimation));
            
            yield break;
        }
    }
}