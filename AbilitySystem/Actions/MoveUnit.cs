using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using SoulStitcher.Scripts.Game.AbilitySystem.Actions.Core;
using SoulStitcher.Scripts.Game.Units;
using UnityEngine;

namespace SoulStitcher.Scripts.Game.AbilitySystem.Actions
{
    [Serializable]
    public class MoveUnit : AbilityAction
    {
        [SerializeField] private AbilityActionTargetType _target;
        [SerializeField] private Vector3 _axis;
        [SerializeField] private Ease _ease;
        [SerializeField] private float _value;
        [SerializeField] private float _duration;

        private List<Tween> _runningTweens;

        public override IEnumerator Execute(AbilityContext context)
        {
            _runningTweens = new List<Tween>();
            
            if (_target == AbilityActionTargetType.Caster)
                Move(context.Caster.Unit);
            else
                foreach (IUnit unit in context.Targets)
                    Move(unit);

            yield break;
        }

        private void Move(IUnit unit)
        {
            Tween tween = unit.Transform.DOMove(unit.Transform.position + _axis * _value, _duration)
                .SetEase(_ease)
                .SetLink(unit.Transform.gameObject);
            
            _runningTweens.Add(tween);
        }
    }
}