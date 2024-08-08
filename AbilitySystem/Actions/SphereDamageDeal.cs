using System;
using System.Collections;
using SoulStitcher.Scripts.Game.AbilitySystem.Actions.Core;
using SoulStitcher.Scripts.Game.DamageSystem;
using UniRx;
using UnityEngine;
using Zenject;

namespace SoulStitcher.Scripts.Game.AbilitySystem.Actions
{
    [Serializable]
    public class SphereDamageDeal : AbilityAction
    {
        private const int CAST_LIMIT = 20;

        [SerializeField] private LayerMask _mask;
        [SerializeField, Min(0)] private int _damage;
        [SerializeField, Min(0)] private float _range;
        [SerializeField, Min(0)] private float _duration;
        [SerializeField, Min(1)] private float _ticksCount;

        private Collider[] _castResults = new Collider[CAST_LIMIT];
        private DamageManager _damageManager;
        private IDisposable _update;

        public override void Initialize(DiContainer container)
        {
            _damageManager = container.Resolve<DamageManager>();
        }
        
        public override IEnumerator Execute(AbilityContext context)
        {
            float tickInterval = _duration / _ticksCount;
            int tickIndex = 0;

            _update = Observable
                .Interval(TimeSpan.FromSeconds(tickInterval))
                .TakeWhile(_ => tickIndex < _ticksCount)
                .Subscribe(_ =>
                {
                    Tick(context);
                    tickIndex++;
                });
            
            yield break;
        }

        private void Tick(AbilityContext context)
        {
            int size = Physics.OverlapSphereNonAlloc(context.Caster.Unit.Transform.position, _range, _castResults, _mask);

            for (int i = 0; i < size; i++)
            {
                if (_castResults[i].TryGetComponent(out IDamageReceiver damageReceiver))
                {
                    Damage damage = new(context.Caster.Unit, damageReceiver, DamageSourceType.Ability, _castResults[i].ClosestPoint(context.Caster.Unit.Transform.position), _damage);
                    _damageManager.HandleDamage(damage);
                }
            }
        }
    }
}