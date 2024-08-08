using System.Collections.Generic;
using JetBrains.Annotations;
using SoulStitcher.Scripts.Game.Services.Global.PoolingSystem;
using UniRx;
using UnityEngine;

namespace SoulStitcher.Scripts.Game.WeaponSystem.Projectiles
{
    public interface IProjectileFactory
    {
        public Projectile GetProjectile(Projectile projectile, Vector3 position, Quaternion rotation, Transform parent = null);
    }
    
    [UsedImplicitly]
    public class ProjectileFactory : IProjectileFactory
    {
        private readonly IPoolingService _poolingService;
        private readonly List<Projectile> _activeProjectiles;

        public ProjectileFactory(IPoolingService poolingService)
        {
            _poolingService = poolingService;
            _activeProjectiles = new List<Projectile>();
            
            CheckConditionProjectiles();
        }

        public Projectile GetProjectile(Projectile projectile, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            Projectile projectileInstance = _poolingService.Get(projectile, position, rotation, parent);
            _activeProjectiles.Add(projectileInstance);
            return projectileInstance;
        }

        private void CheckConditionProjectiles()
        {
            Observable.EveryUpdate()
                .Subscribe(_ =>
                {
                    for (int i = _activeProjectiles.Count - 1; i >= 0; i--)
                    {
                        if (_activeProjectiles[i].gameObject.activeSelf)
                            continue;
                
                        _poolingService.Return(_activeProjectiles[i]);
                        _activeProjectiles.RemoveAt(i);
                    }
                });
        }
    }
}