using System;

namespace SoulStitcher.Scripts.Game.DamageSystem
{
    public interface IDamageReceiver
    {
        public TeamTag TeamTag { get; }
        public event Action<int, bool> DamageReceived;
        public void ReceiveDamage(int damage, bool isCritical);
    }
}