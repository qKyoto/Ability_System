using System;
using System.Collections;
using SoulStitcher.Scripts.Game.AbilitySystem.Actions.Core;
using UnityEngine;

namespace SoulStitcher.Scripts.Game.AbilitySystem.Actions
{
    [Serializable]
    public class DashCharacterController : AbilityAction
    {
        [SerializeField] private LayerMask _obstacleMask;
        [SerializeField] private Vector3 _raycastOffset;
        [SerializeField, Min(0)] private float _speed;
        [SerializeField, Min(0)] private float _dashDistance;

        public override IEnumerator Execute(AbilityContext context)
        {
            CharacterController characterController = context.Caster.Unit.Transform.GetComponent<CharacterController>();
            characterController.enabled = false;

            float distance = GetDistance(context.Caster.Unit.Transform.position, context.Caster.Unit.Transform.forward);
            float maxDashTime = distance / _speed;
            float currentDashTime = 0;

            while (currentDashTime < maxDashTime)
            {
                context.Caster.Unit.Transform.Translate(context.Caster.Unit.Transform.forward * _speed * Time.deltaTime, Space.World);
                currentDashTime += Time.deltaTime;
                yield return null;
            }
            
            characterController.enabled = true;
        }

        private float GetDistance(Vector3 casterPosition, Vector3 casterDirection)
        {
            float distance = _dashDistance;
            
            if (Physics.Raycast(casterPosition + _raycastOffset, casterDirection, out RaycastHit hit, _dashDistance, _obstacleMask))
                distance = hit.distance;

            return distance;
        }
    }
}