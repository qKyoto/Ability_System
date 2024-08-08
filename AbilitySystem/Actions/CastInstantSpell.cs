using System;
using NaughtyAttributes;
using SoulStitcher.Scripts.Game.AbilitySystem.Actions.Core;
using SoulStitcher.Scripts.Game.DamageSystem;
using SoulStitcher.Scripts.Game.Units;
using SoulStitcher.Scripts.Game.WeaponSystem;
using SoulStitcher.Scripts.Game.WeaponSystem.Projectiles;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace SoulStitcher.Scripts.Game.AbilitySystem.Actions
{
    public interface ISpell
    {
        public void Cast(IUnit owner);
    }

    public class DamageRing : ISpell
    {
        [SerializeField] private int _damage;
        
        private const int RING_AMOUNT = 3;
        
        public void Cast(IUnit owner)
        {
        }
    }
    
    [Serializable]
    public class ProjectileLauncher : ISpell
    {
        [SerializeField] private Projectile _projectilePrefab;
        [SerializeField] private Vector3 _startOffset;
        [SerializeField, MinValue(0), MaxValue(360)] private float _angel;
        [SerializeField, MinValue(0)] private int _count;
        [SerializeField] private int _damage;
        [SerializeField] private float _rotationOffset;

        public void Initialize(Damage damage)
        {
            
        }
        
        public void Cast(IUnit owner)
        {
            float angleStep = _angel / (_count - 1);
            float angle = -(_angel / 2);
            Vector3 startPosition = owner.Transform.position + _startOffset;

            for (int i = 0; i < _count; i++)
            {
                Quaternion targetRotation = owner.Transform.rotation * Quaternion.AngleAxis(angle, Vector3.up);
                angle += angleStep;
                
                //TODO: need replace on object pool
                Projectile projectile = Object.Instantiate(_projectilePrefab, startPosition, Quaternion.identity);
                Damage damage = new(owner, null, DamageSourceType.Ability, Vector3.zero, _damage);
                projectile.Initialize(damage);

#if UNITY_EDITOR
                Debug.DrawRay(startPosition, targetRotation * Vector3.forward, Color.cyan, 5);
#endif
            }
        }
    }
}