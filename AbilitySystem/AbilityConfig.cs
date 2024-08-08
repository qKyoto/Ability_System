using System;
using System.Collections.Generic;
using NaughtyAttributes;
using SoulStitcher.Scripts.Game.AbilitySystem.Actions;
using SoulStitcher.Scripts.Game.AbilitySystem.Actions.Core;
using SoulStitcher.Scripts.Game.StatsSystem;
using SoulStitcher.Scripts.Utilities.RuntimeUtility.SubClassSelector;
using UnityEngine;

namespace SoulStitcher.Scripts.Game.AbilitySystem
{
    public enum CooldownType
    {
        Time,
        AutoAttackHit
    }

    public enum InputActivationType
    {
        OnPressing,
        OnRelease
    }

    public enum AbilityDirectionType
    {
        NonTarget,
        Point,
        Aoe
    }

    [Flags]
    public enum TargetTypeFlags
    {
        Enemy = 1,
        Ally = 2
    }
    
    [CreateAssetMenu(fileName = "AbilityConfig", menuName = "Ability/Ability Config")]
    public class AbilityConfig : ScriptableObject
    {
        [SerializeField] private CasterType _caster;
        [SerializeField, ShowIf(nameof(_caster), CasterType.Player)] private InputActivationType _triggering;
        [SerializeField] private AbilityDirectionType _direction;
        [SerializeField, ShowIf(nameof(_direction), AbilityDirectionType.Aoe)] private TargetTypeFlags _targets;
        [SerializeField] private CooldownType _cooldown;
        [Space(10)]
        [SerializeField, ShowIf(nameof(_direction), AbilityDirectionType.Aoe)] private LayerMask _mask;
        [SerializeField, ShowIf(nameof(_direction), AbilityDirectionType.Aoe)] private float _range;
        [SerializeField, ShowIf(nameof(_direction), AbilityDirectionType.Aoe)] private float _distanceFromCaster;
        [Space(10)]
        [SerializeField, Min(0), ShowIf(nameof(_cooldown), CooldownType.Time)] private float _cooldownTime;
        [Space(10)]
        [SerializeField, Min(0), ShowIf(nameof(_cooldown), CooldownType.AutoAttackHit)] private int _hits;
        [Space(10)]
        [SerializeReference, SubclassSelector(typeof(IAbilityAction)), ShowIf(nameof(HasPreCastActions))] private IAbilityAction[] _preCastActions;
        [SerializeReference, SubclassSelector(typeof(IAbilityAction))] private IAbilityAction[] _castActions;

        public InputActivationType Triggering => _triggering;
        public AbilityDirectionType Direction => _direction;
        public TargetTypeFlags Targets => _targets; 
        public CooldownType Cooldown => _cooldown;
        public LayerMask AoeMask => _mask;
        public float AoeRange => _range;
        public float AoeDistanceFromCaste => _distanceFromCaster;
        public float CooldownTime => _cooldownTime;
        public int Hits => _hits;
        public IEnumerable<IAbilityAction> PreCastActions => _preCastActions;
        public IEnumerable<IAbilityAction> CastActions => _castActions;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!HasPreCastActions())
                _preCastActions = new IAbilityAction[] {};
            if (_caster == CasterType.Enemy && _cooldown == CooldownType.AutoAttackHit)
                _cooldown = CooldownType.Time;
        }

        private bool HasPreCastActions()
        {
            return _caster == CasterType.Player && _triggering == InputActivationType.OnRelease;
        }
#endif
        
        private enum CasterType
        {
            Player,
            Enemy
        }
    }
}