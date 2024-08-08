using System.Collections.Generic;
using JetBrains.Annotations;
using SoulStitcher.Scripts.Game.DamageSystem;
using SoulStitcher.Scripts.Game.Units;
using UnityEngine;
using Zenject;

namespace SoulStitcher.Scripts.Game.AbilitySystem
{
    public interface IAbilityFactory
    {
        public Ability GetAbility(AbilityCaster caster, AbilityConfig config);
        public AbilityContext GetContext(IAbilityCaster caster, AbilityConfig config, Vector3 point);
    }
    
    [UsedImplicitly]
    public class AbilityFactory : IAbilityFactory
    {
        private const int AOE_LIMIT = 10;
        
        private readonly Collider[] _detectedColliders = new Collider[AOE_LIMIT];
        private readonly IInstantiator _instantiator;

        public AbilityFactory(IInstantiator instantiator)
        {
            _instantiator = instantiator;
        }

        public Ability GetAbility(AbilityCaster caster, AbilityConfig config)
        {
            return _instantiator.Instantiate<Ability>(extraArgs: new Object[]{ caster, config });
        }
        
        public AbilityContext GetContext(IAbilityCaster caster, AbilityConfig config, Vector3 point)
        {
            List<IUnit> units = new();

            if (config.Direction == AbilityDirectionType.Aoe)
                units = GetUnitsInAoeZone(point, config.AoeRange, config.AoeMask, caster.Unit.TeamTag, config.Targets);

            AbilityContext context = new(units, caster, caster.Unit.Transform.forward, caster.Unit.Transform.position, point);
            
            return context;
        }

        private List<IUnit> GetUnitsInAoeZone(Vector3 point, float range, LayerMask mask, TeamTag casterTag, TargetTypeFlags targetFlags)
        {
            List<IUnit> units = new();

            int size = Physics.OverlapSphereNonAlloc(point, range, _detectedColliders, mask);

            for (int i = 0; i < size; i++)
            {
                if (_detectedColliders[i].TryGetComponent(out IUnit unit))
                {
                    if (unit.TeamTag == casterTag)
                    {
                        if (targetFlags.HasFlag(TargetTypeFlags.Ally))
                            units.Add(unit);
                    }
                    else
                    {
                        if (targetFlags.HasFlag(TargetTypeFlags.Enemy))
                            units.Add(unit);
                    }
                }
            }

            return units;
        }
    }
}