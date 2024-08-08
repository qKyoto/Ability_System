using System;
using NaughtyAttributes;
using SoulStitcher.Scripts.Game.AutoAttackSystem.Core;
using SoulStitcher.Scripts.Game.DamageSystem;
using UnityEngine;
using Zenject;

namespace SoulStitcher.Scripts.Game.AutoAttackSystem.Attacks
{
    [Serializable]
    public class MeleeNonWeaponAttack : AutoAttack
    {
        private const int TARGET_LIMIT = 10;
        
        [HorizontalLine]
        [SerializeField] private LayerMask _mask;
        [SerializeField] private Vector3 _sphereCastOffset;
        [SerializeField, Min(0)] private float _range;

        private Collider[] _detectedColliders = new Collider[TARGET_LIMIT];
        
        public override void Initialize(DiContainer diContainer)
        {
            EventReceiver.DamageReceived += TryReceiveDamage;
            base.Initialize(diContainer);
        }

        public override void Dispose()
        {
            EventReceiver.DamageReceived -= TryReceiveDamage;
            base.Dispose();
        }

        private void TryReceiveDamage()
        {
            Vector3 castPosition = Unit.Transform.TransformPoint(_sphereCastOffset);
            int size = Physics.OverlapSphereNonAlloc(castPosition, _range, _detectedColliders, _mask);

            for (int i = 0; i < size; i++)
            {
                if (_detectedColliders[i].TryGetComponent(out IDamageReceiver damageReceiver))
                {
                    (int damage, bool isCritical) calculatedDamage = CalculateDamage();
                    Damage damage = new(Unit, damageReceiver, DamageSourceType.AutoAttack,_detectedColliders[i].ClosestPoint(Unit.Transform.position), calculatedDamage.damage, calculatedDamage.isCritical);
                    DamageManager.HandleDamage(damage);
                }
            }
        }
        
        public override void DrawGizmos()
        {
            Gizmos.DrawWireSphere(Unit.Transform.TransformPoint(_sphereCastOffset), _range);
        }
    }
}