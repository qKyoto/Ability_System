using System;
using System.Collections;
using SoulStitcher.Scripts.Game.AbilitySystem.Actions.Core;
using SoulStitcher.Scripts.Game.Units.EnemiesComponents;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace SoulStitcher.Scripts.Game.AbilitySystem.Actions
{
    [Serializable]
    public class TeleportAIUnit : AbilityAction
    {
        private const int ATTEMPTS_COUNT = 100;
        
        [SerializeField] private float _undergroundPosition = -2f;
        [SerializeField] private float _teleportNearPlayerRadius = 7f;
        
        public override IEnumerator Execute(AbilityContext context)
        {
            EnemyAIBase casterUnit = context.Caster.Unit.Transform.GetComponent<EnemyAIBase>();

            if (TryGetTeleportPoint(context, out Vector3 point))
                casterUnit.Transform.position = new Vector3(point.x, _undergroundPosition, point.z);
            
            yield break;
        }
        
        private bool TryGetTeleportPoint(AbilityContext context, out Vector3 point)
        {
            Vector3 playerPoint = context.TargetPoint;

            int count = 0;
            while (count < ATTEMPTS_COUNT)
            {
                Vector3 randomPoint = Random.onUnitSphere * _teleportNearPlayerRadius + playerPoint;
                if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, _teleportNearPlayerRadius,NavMesh.AllAreas))
                {
                    point = hit.position;
                    return true;
                }
                count++;
            }

            point = Vector3.zero;
            return false;
        }
    }
}