using System;
using System.Collections.Generic;
using NaughtyAttributes;
using SoulStitcher.Scripts.Game.AbilitySystem.StatusEffects;
using SoulStitcher.Scripts.Game.DamageSystem;
using SoulStitcher.Scripts.Game.Units;
using UniRx;
using UnityEngine;
using Zenject;

namespace SoulStitcher.Scripts.Game.WeaponSystem.Projectiles
{
    [Serializable]
    public class ProjectileConfig
    {
        [SerializeField] private OptionFlags _options;
        [SerializeField, ShowIf(nameof(CanDealDamage)), AllowNesting] private int _defaultDamage;
        [SerializeField, ShowIf(nameof(CanApplyStatusEffects)), AllowNesting] private List<StatusEffect> _statusEffects;
        [HorizontalLine]
        [SerializeField] private float _speed;
        [SerializeField] private float _lifeTime;

        public bool CanDealDamage => _options.HasFlag(OptionFlags.Damage);
        public bool CanApplyStatusEffects => _options.HasFlag(OptionFlags.StatusEffects);
        public float LifeTime => _lifeTime;
        public float Speed => _speed;
        public int DefaultDamage => _defaultDamage;
        public IEnumerable<StatusEffect> StatusEffects => _statusEffects;
        
        [Flags]
        private enum OptionFlags
        {
            Damage = 1,
            StatusEffects = 2
        }
    }

    public abstract class Projectile : MonoBehaviour
    {
        [SerializeField] private ProjectileConfig _config;

        private IStatusEffectsManager _effectsManager;
        private DamageManager _damageManager;
        private IDisposable _update;
        private Damage _damage;

        protected float Speed => _config.Speed;
        
        [Inject]
        private void Construct(
            IStatusEffectsManager effectsManager,
            DamageManager damageManager)
        {
            _effectsManager = effectsManager;
            _damageManager = damageManager;
        }

        public void Initialize(Damage damage)
        {
            if (_update != null)
                return;
            
            _damage = damage;
            Launch();
        }

        protected abstract void Move();
        
        private void Launch()
        {
            float currentLifeTime = 0;
            
            _update = Observable.EveryUpdate()
                .TakeWhile(_ => currentLifeTime < _config.LifeTime)
                .Finally(Disable)
                .Subscribe(_ =>
                {
                    Move();
                    currentLifeTime += Time.deltaTime;
                });
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out IUnit unit))
            {
                if (unit.TeamTag == _damage.Dealer.TeamTag)
                    return;

                Debug.Log(_config.CanApplyStatusEffects);
                if (_config.CanApplyStatusEffects)
                {
                    foreach (StatusEffect statusEffect in _config.StatusEffects)
                        _effectsManager.ApplyStatusEffect(unit, statusEffect);
                }
            }
            
            if (other.TryGetComponent(out IDamageReceiver damageReceiver) && _config.CanDealDamage)
            {
                _damage.HitPoint = other.ClosestPoint(transform.position);
                _damage.Receiver = damageReceiver;
                _damage.Amount += _config.DefaultDamage;
                
                _damageManager.HandleDamage(_damage);
            }
            
            _update?.Dispose();
            Disable();
        }

        private void Disable()
        {
            _update = null;
            gameObject.SetActive(false);
        }
    }
}