using System;
using System.Collections;
using SoulStitcher.Scripts.Game.AbilitySystem.Actions.Core;
using SoulStitcher.Scripts.Game.Units;
using SoulStitcher.Scripts.Game.Units.EnemiesComponents;
using UnityEngine;

namespace SoulStitcher.Scripts.Game.AbilitySystem.Actions
{
    [Serializable]
    public class SwitchAINavMeshState : AbilityAction
    {
        [SerializeField] private NavMeshState _navMeshState;
        [SerializeField] private AbilityActionTargetType _target;
        
        public override IEnumerator Execute(AbilityContext context)
        {
            if (_target == AbilityActionTargetType.Caster)
                SwitchNavMesh(context.Caster.Unit);
            else
                foreach (IUnit unit in context.Targets)
                    SwitchNavMesh(unit);
            
            yield break;
        }

        private void SwitchNavMesh(IUnit unit)
        {
            if (unit.Transform.TryGetComponent(out EnemyAIBase aiBase))
                aiBase.NavMeshAgent.enabled = GetStateFlag();
        }
        
        private bool GetStateFlag()
        {
            return _navMeshState == NavMeshState.Enable;
        }
        
        private enum NavMeshState
        {
            Enable,
            Disable
        }
    }
}