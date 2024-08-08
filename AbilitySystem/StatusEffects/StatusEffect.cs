using System;
using System.Collections.Generic;
using NaughtyAttributes;
using SoulStitcher.Scripts.Game.AbilitySystem.Actions;
using SoulStitcher.Scripts.Game.AbstractFactories;
using SoulStitcher.Scripts.Game.StatsSystem;
using SoulStitcher.Scripts.Game.Units.Interaction;
using SoulStitcher.Scripts.Game.Units.Modifiers;
using UniRx;
using UnityEditor;
using UnityEngine;
using Zenject;
using Attribute = SoulStitcher.Scripts.Game.StatsSystem.Attribute;

namespace SoulStitcher.Scripts.Game.AbilitySystem.StatusEffects
{
    public enum StatusEffectDurationType
    {
        Time,
        Permanent
    }
    
    [CreateAssetMenu(fileName = "Status Effect", menuName = "Ability/Status Effect")]
    public class StatusEffect : ScriptableObject
    {
        [SerializeField] private StatusEffectType _effectType;
        [SerializeField] private ExecutionType _executionType;
        
        [Header("Duration"), HorizontalLine(2, EColor.Blue)]
        [SerializeField] private StatusEffectDurationType _durationType;
        [SerializeField, ShowIf(nameof(_durationType), StatusEffectDurationType.Time), AllowNesting] private float _duration;
        [SerializeField, ShowIf(nameof(HasTicks))] private int _ticks;

        [Header("Modification"), HorizontalLine(2, EColor.Blue)]
        [SerializeField] private ModificationType _modificationType;
        [SerializeField, ShowIf(nameof(_modificationType), ModificationType.Stat)] private StatModifier _statModifier;
        [SerializeField, ShowIf(nameof(_modificationType), ModificationType.Attribute)] private AttributeType _attributeType;
        [SerializeField, ShowIf(nameof(_modificationType), ModificationType.Attribute)] private float _attributeValue;
        [SerializeField, ShowIf(nameof(_modificationType), ModificationType.UnitState)] private UnitModifierFlags _modifiers;
        
        [Header("Vfx"), HorizontalLine(2, EColor.Blue)]
        [SerializeField] private bool _hasVfx;
        [SerializeField, ShowIf(nameof(_hasVfx)), AllowNesting] private UnitInteractionPointType _interactionPoint;
        [SerializeField, ShowIf(nameof(_hasVfx)), AllowNesting] private ParticleSystem _vfx;

        private List<ParticleSystem> _playableVfx;
        private IVfxFactory _vfxFactory;
        private IDisposable _update;
        private float _runtimeDuration;
        private float _lifeTime;

        private bool HasTicks => _durationType == StatusEffectDurationType.Time && _executionType == ExecutionType.Tick;

        public float Duration => _duration;
        public StatusEffectType StatusEffectType => _effectType;
        public StatusEffectDurationType DurationType => _durationType;
        public UnitModifierFlags Modifiers => _modifiers;

        public event Action<IEffectable, StatusEffectType> Finalized;
        public event Action Cleaned;

        [Inject]
        private void Construct(IVfxFactory vfxFactory)
        {
            _vfxFactory = vfxFactory;
        }
        
        public void CleanUp()
        {
            _update?.Dispose();
            RemoveVfx();
            Cleaned?.Invoke();
        }

        public void Apply(IEffectable unit)
        {
            ResetDuration(_duration);
            ApplyVfx(unit);
            Execute(unit);
        }

        public void Finalize(IEffectable effectable)
        {
            if (_modificationType == ModificationType.Stat)
                effectable.StatsSheet.GetStat(_statModifier.StatType).RemoveModifier(_statModifier);
            
            Finalized?.Invoke(effectable, StatusEffectType);
        }

        public void ResetDuration(float duration)
        {
            _lifeTime = 0;
            _runtimeDuration = duration;
        }

        private void Execute(IEffectable unit)
        {
            if (_durationType == StatusEffectDurationType.Time && _executionType == ExecutionType.Tick && _modificationType == ModificationType.Attribute)
            {
                Attribute attribute = unit.StatsSheet.GetAttribute(_attributeType);
                float tickInterval = _runtimeDuration / _ticks;
                float accumulatedTime = 0;
                
                _update = Observable.EveryUpdate()
                    .TakeWhile(_ => _lifeTime < _runtimeDuration)
                    .Finally(() =>
                    {
                        Finalize(unit);
                    })
                    .Subscribe(_ =>
                    {
                        _lifeTime += Time.deltaTime;
                        accumulatedTime += Time.deltaTime;

                        if (accumulatedTime >= tickInterval)
                        {
                            accumulatedTime = 0;
                            attribute.Value += _attributeValue;
                        }
                    });
            }
            else if (_durationType == StatusEffectDurationType.Time && _executionType == ExecutionType.Single)
            {
                if (_modificationType == ModificationType.Stat)
                    unit.StatsSheet.GetStat(_statModifier.StatType).AddModifier(_statModifier);
                
                _update = Observable.EveryUpdate()
                    .TakeWhile(_ => _lifeTime < _runtimeDuration)
                    .Finally(() =>
                    { 
                        Finalize(unit);
                    })
                    .Subscribe(_ =>
                    {
                        _lifeTime += Time.deltaTime;
                    });
            }
        }

        private void ApplyVfx(IEffectable unit)
        {
            if (!_hasVfx)
                return;
            
            _playableVfx = new List<ParticleSystem>();
            foreach (InteractionPoint interactionPoint in unit.InteractionPoints.GetPoints(_interactionPoint))
            {
                ParticleSystem vfxInstance = _vfxFactory.GetVfx(_vfx, interactionPoint.transform.position, Quaternion.identity, interactionPoint.Size , interactionPoint.transform);
                vfxInstance.Play();
                _playableVfx.Add(vfxInstance);
            }
        }

        private void RemoveVfx()
        {
            if (!_hasVfx)
                return;

            for (int i = _playableVfx.Count - 1; i >= 0; i--)
            {
                _playableVfx[i].Stop();
                _playableVfx.RemoveAt(i);
            }
        }
        
        private enum ModificationType
        {
            Attribute,
            Stat,
            UnitState
        }
        
        private enum ExecutionType
        {
            Single,
            Tick
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_modificationType != ModificationType.UnitState)
                _modifiers = 0;

            if (_durationType == StatusEffectDurationType.Permanent)
            {
                _executionType = ExecutionType.Single;

                if (_modificationType == ModificationType.Attribute)
                    _modificationType = ModificationType.Stat;
            }
        }
#endif
    }
}