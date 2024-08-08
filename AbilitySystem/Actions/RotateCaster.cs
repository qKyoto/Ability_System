using System;
using System.Collections;
using SoulStitcher.Scripts.Game.AbilitySystem.Actions.Core;
using UniRx;
using UnityEngine;

namespace SoulStitcher.Scripts.Game.AbilitySystem.Actions
{
    [Serializable]
    public class RotateCaster : AbilityAction
    {
        [SerializeField] private Vector3 _axis;
        [SerializeField, Min(0)] private float _speed;
        [SerializeField, Min(0)] private float _duration;

        private IDisposable _update;

        public override IEnumerator Execute(AbilityContext context)
        {
            float rotationTime = 0;

            _update = Observable.EveryUpdate()
                .TakeWhile(_ => rotationTime < _duration)
                .Subscribe(_ =>
                {
                    context.Caster.Unit.Transform.Rotate(_axis * _speed * Time.deltaTime);
                    rotationTime += Time.deltaTime;
                });
            
            yield break;
        }
    }
}