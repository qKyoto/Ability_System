using UnityEngine;

namespace SoulStitcher.Scripts.Game.WeaponSystem.Projectiles
{
    public class DirectionalProjectile : Projectile
    {
        protected override void Move()
        {
            transform.Translate(transform.forward * (Speed * Time.deltaTime), Space.World);
        }
    }
}