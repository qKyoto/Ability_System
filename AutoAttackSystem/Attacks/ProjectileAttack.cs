using System;
using System.Collections.Generic;
using NaughtyAttributes;
using SoulStitcher.Scripts.Game.AutoAttackSystem.Core;
using SoulStitcher.Scripts.Game.DamageSystem;
using SoulStitcher.Scripts.Game.WeaponSystem.Projectiles;
using UnityEngine;
using Zenject;

namespace SoulStitcher.Scripts.Game.AutoAttackSystem.Attacks
{
    [Serializable]
    public class ProjectileAttack : AutoAttack
    {
        [HorizontalLine]
        [SerializeField] private Projectile _projectilePrefab;
        [SerializeField] private Vector3 _startOffset;
        [SerializeField, MinValue(0), MaxValue(360)] private float _angle;
        [SerializeField, MinValue(0)] private int _count;
        [Space(10)]
        [SerializeField] private bool _hasVisualizer;
        [SerializeField, ShowIf(nameof(_hasVisualizer)), AllowNesting] private DirectionalAttackVisualizer _visualizer;

        private IProjectileFactory _projectileFactory;
        private IAttackVisualizerFactory _visualizerFactory;
        private List<DirectionalAttackVisualizer> _visualizers;
        
        public override void Initialize(DiContainer diContainer)
        {
            EventReceiver.DamageReceived += LaunchProjectile;
            _projectileFactory = diContainer.Resolve<IProjectileFactory>();
            _visualizerFactory = diContainer.Resolve<IAttackVisualizerFactory>();
            _visualizers = new List<DirectionalAttackVisualizer>();
            base.Initialize(diContainer);
        }

        public override void Dispose()
        {
            EventReceiver.DamageReceived -= LaunchProjectile;
            base.Dispose();
        }

        protected override void OnPrePerformed()
        {
            if (_hasVisualizer)
                ShowAttackVisualisers();
        }

        protected override void OnPerformed()
        {
            if (_hasVisualizer)
                HideAttackVisualisers();
        }

        private void LaunchProjectile()
        {
            (float angleStep, float angle) calculatedAngle = CalculateAngleStep();
            (int damage, bool isCritical) calculatedDamage = CalculateDamage();
            Vector3 startPosition = Unit.Transform.TransformPoint(_startOffset);

            for (int i = 0; i < _count; i++)
            {
                Quaternion targetRotation = GetTargetRotation(calculatedAngle.angle);
                calculatedAngle.angle += calculatedAngle.angleStep;

                Projectile projectile = _projectileFactory.GetProjectile(_projectilePrefab, startPosition, targetRotation);
                Damage damage = new(Unit, null, DamageSourceType.AutoAttack, Vector3.zero, calculatedDamage.damage, calculatedDamage.isCritical);
                projectile.Initialize(damage);
                
#if UNITY_EDITOR
                Debug.DrawRay(startPosition, targetRotation * Vector3.forward, Color.cyan, 5);
#endif
            }
        }

        private (float, float) CalculateAngleStep()
        {
            float angleStep = _angle / (_count - 1);
            float angle = -(_angle / 2);
            (float, float) result = (angleStep, angle);
            
            return result;
        }

        private Quaternion GetTargetRotation(float angle)
        {
            return Unit.Transform.rotation * Quaternion.AngleAxis(angle, Vector3.up);
        }

        private void ShowAttackVisualisers()
        {
            (float angleStep, float angle) calculatedAngle = CalculateAngleStep();
                
            for (int i = 0; i < _count; i++)
            {
                Quaternion targetRotation = GetTargetRotation(calculatedAngle.angle);
                calculatedAngle.angle += calculatedAngle.angleStep;
                    
                DirectionalAttackVisualizer attackVisualizer = _visualizerFactory.GetDirectionalVisualizer(_visualizer, Unit.Transform.position, targetRotation, Unit.Transform);
                
                _visualizers.Add(attackVisualizer);
            }
        }

        private void HideAttackVisualisers()
        {
            for (int i = _visualizers.Count - 1; i >= 0; i--)
            {
                _visualizers[i].gameObject.SetActive(false);
                _visualizers.RemoveAt(i);
            }
        }
        
        public override void DrawGizmos()
        {
            Gizmos.DrawWireSphere(Unit.Transform.TransformPoint(_startOffset), 0.3f);
        }
    }
}