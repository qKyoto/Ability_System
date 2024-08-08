using System;

namespace SoulStitcher.Scripts.Game.DamageSystem
{
    public interface IDamageDealer
    {
        public TeamTag TeamTag { get; }
        public event Action<int, DamageSourceType, TeamTag> DamageProcessed;
        public void ProcessDamage(int damage, DamageSourceType damageSourceType, TeamTag receiverTag);
    }
}