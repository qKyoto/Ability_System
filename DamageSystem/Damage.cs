using UnityEngine;

namespace SoulStitcher.Scripts.Game.DamageSystem
{
    public struct Damage
    {
        public IDamageReceiver Receiver;
        public Vector3 HitPoint;
        public int Amount;
        
        public IDamageDealer Dealer { get; }
        public DamageSourceType DamageSourceType { get; }
        public bool IsCritical { get; }

        public Damage(
            IDamageDealer dealer,
            IDamageReceiver receiver,
            DamageSourceType damageSourceType,
            Vector3 hitPoint,
            int amount,
            bool isCritical = false)
        {
            Dealer = dealer;
            Receiver = receiver;
            DamageSourceType = damageSourceType;
            HitPoint = hitPoint;
            Amount = amount;
            IsCritical = isCritical;
        }
    }
}