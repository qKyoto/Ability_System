using System;
using System.Collections.Generic;
using SoulStitcher.Scripts.Game.Units;
using SoulStitcher.Scripts.Game.Units.Modifiers;
using UnityEngine;
using Zenject;

namespace SoulStitcher.Scripts.Game.AbilitySystem
{
    public interface IAbilityCaster
    {
        public IUnit Unit { get; }
    }
    
    public abstract class AbilityCaster : MonoBehaviour, IAbilityCaster
    {
        [SerializeField] private AbilityConfig[] _abilityConfigs;
        [SerializeField] private Unit _unit;
        
        private Dictionary<AbilityConfig, Ability> _abilities;
        
        protected Ability ActiveAbility;

        protected IAbilityFactory AbilityFactory { get; private set; }
        protected IReadOnlyDictionary<AbilityConfig, Ability> Abilities => _abilities;
        
        public IUnit Unit => _unit;
        public bool IsCasting => ActiveAbility is { State: AbilityStateType.Casting or AbilityStateType.PreCast };

        [Inject]
        private void Construct(IAbilityFactory abilityFactory)
        {
            AbilityFactory = abilityFactory;
        }

        private void Awake()
        {
            InitializeAbilities();
        }
        
        public void Cast(AbilityConfig config, Action<bool> abilityPerformed = null)
        {
            if (!CanCast(config))
            {
                abilityPerformed?.Invoke(false);
                return;
            }

            AbilityContext context = AbilityFactory.GetContext(this, config, GetTargetPoint(config));
            ActiveAbility = _abilities[config];
            ActiveAbility.Performed += OnAbilityPerformed;
            ActiveAbility.Cast(context);

            void OnAbilityPerformed()
            {
                ActiveAbility.Performed -= OnAbilityPerformed;
                ActiveAbility = null;
                
                abilityPerformed?.Invoke(true);
            }
        }

        public void Cancel()
        {
            ActiveAbility?.Cancel();
            ActiveAbility = null;
        }

        public virtual bool CanCast(AbilityConfig config)
        {
            if (Unit.UnitFlags.HasFlag(UnitModifierFlags.BlockAbilityCast))
                return false;
            if (ActiveAbility is { State: AbilityStateType.Casting or AbilityStateType.Cooldown })
                return false;
            if (Abilities[config].State != AbilityStateType.Ready &&
                Abilities[config].State != AbilityStateType.PreCast)
                return false;
            
            return true;
        }

        protected virtual Vector3 GetTargetPoint(AbilityConfig config)
        {
            return transform.position;
        }

        private void InitializeAbilities()
        {
            _abilities = new Dictionary<AbilityConfig, Ability>();

            foreach (AbilityConfig abilityConfig in _abilityConfigs)
                _abilities.Add(abilityConfig, AbilityFactory.GetAbility(this, abilityConfig));
        }
    }
}