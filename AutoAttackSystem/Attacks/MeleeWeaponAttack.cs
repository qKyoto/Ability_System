using System;
using System.Collections.Generic;
using NaughtyAttributes;
using SoulStitcher.Scripts.Game.AutoAttackSystem.Core;
using SoulStitcher.Scripts.Game.DamageSystem;
using SoulStitcher.Scripts.Game.WeaponSystem;
using UniRx;
using UnityEngine;
using Zenject;

namespace SoulStitcher.Scripts.Game.AutoAttackSystem.Attacks
{
    [Serializable]
    public class MeleeWeaponAttack : AutoAttack
    {
        [HorizontalLine]
        [SerializeField] private Weapon _weapon;
        [SerializeField] private LayerMask _mask;

        private List<Collider> _verifiedColliders = new();
        private IDisposable _update;

        public override void Initialize(DiContainer diContainer)
        {
            EventReceiver.DamageCapabilityChanged += OnDamageCapabilityChanged;
            AttackStarted += OnAttackStarted;
            base.Initialize(diContainer);
        }
        
        public override void Dispose()
        {
            EventReceiver.DamageCapabilityChanged -= OnDamageCapabilityChanged;
            AttackStarted -= OnAttackStarted;
            StopDealDamage();
            base.Dispose();
        }

        private void OnAttackStarted()
        {
            StopDealDamage();
        }
        
        private void OnDamageCapabilityChanged(bool isDamageAvailable)
        {
            if (isDamageAvailable)
                TryDealDamage();
            else
                StopDealDamage();
        }
        
        private void TryDealDamage()
        {
            List<Vector3> previousPoints = _weapon.WeaponRaycastPointsHolder.GetRaycastPoints();
            
            _update?.Dispose();
            _update = Observable
                .EveryUpdate()
                .Subscribe(_ =>
                {
                    List<Vector3> currentPoints = _weapon.WeaponRaycastPointsHolder.GetRaycastPoints();
                    
                    for (int i = 0; i < previousPoints.Count; i++)
                    {
                        if (Physics.Raycast(previousPoints[i], currentPoints[i] - previousPoints[i], out RaycastHit hit, Vector3.Distance(previousPoints[i],currentPoints[i]), _mask))
                        {
                            if (!_verifiedColliders.Contains(hit.collider))
                            {
                                _verifiedColliders.Add(hit.collider);

                                if (hit.collider.TryGetComponent(out IDamageReceiver damageReceiver))
                                {
                                    (int damage, bool isCritical) calculatedDamage = CalculateDamage();
                                    DamageManager.HandleDamage(new Damage(Unit, damageReceiver, DamageSourceType.AutoAttack, hit.point, calculatedDamage.damage, calculatedDamage.isCritical));
                                }
                            }
                        }

#if UNITY_EDITOR
                        Debug.DrawRay(previousPoints[i], currentPoints[i] - previousPoints[i], Color.cyan, 5);
#endif
                    }
                    
                    previousPoints = _weapon.WeaponRaycastPointsHolder.GetRaycastPoints();
                });
        }
        
        private void StopDealDamage()
        {
            _update?.Dispose();
            _verifiedColliders = new List<Collider>();
        }
    }
}