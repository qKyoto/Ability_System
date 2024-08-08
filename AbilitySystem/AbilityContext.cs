using System.Collections.Generic;
using JetBrains.Annotations;
using SoulStitcher.Scripts.Game.Units;
using UnityEngine;

namespace SoulStitcher.Scripts.Game.AbilitySystem
{
    [UsedImplicitly]
    public class AbilityContext
    {
        private readonly List<IUnit> _targets;

        public IEnumerable<IUnit> Targets => _targets;
        public IAbilityCaster Caster { get; }
        public Vector3 Direction { get; }
        public Vector3 CastPoint { get; }
        public Vector3 TargetPoint { get; }

        public AbilityContext(
            List<IUnit> targets, 
            IAbilityCaster caster, 
            Vector3 direction, 
            Vector3 castPoint,
            Vector3 targetPoint)
        {
            _targets = targets;
            Caster = caster;
            Direction = direction;
            CastPoint = castPoint;
            TargetPoint = targetPoint;
        }
    }
}