using SoulStitcher.Scripts.Game.AbilitySystem;
using SoulStitcher.Scripts.Game.AutoAttackSystem.Core;
using UnityEngine;

namespace SoulStitcher.Scripts.Game.WeaponSystem
{
    public abstract class Weapon : MonoBehaviour
    {
        [SerializeField] private AttackEventReceiver _eventReceiver;
        [SerializeField] private WeaponRaycastPointsHolder _weaponRaycastPointsHolder;

        public AttackEventReceiver EventReceiver => _eventReceiver;
        public WeaponRaycastPointsHolder WeaponRaycastPointsHolder => _weaponRaycastPointsHolder;
    }
}