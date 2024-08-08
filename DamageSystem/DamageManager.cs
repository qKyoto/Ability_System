using JetBrains.Annotations;

namespace SoulStitcher.Scripts.Game.DamageSystem
{
    [UsedImplicitly]
    public class DamageManager
    {
        public void HandleDamage(Damage damage)
        {
            if (damage.Dealer.TeamTag == damage.Receiver.TeamTag)
                return;
            
            damage.Dealer.ProcessDamage(damage.Amount, damage.DamageSourceType, damage.Receiver.TeamTag);
            damage.Receiver.ReceiveDamage(damage.Amount, damage.IsCritical);
        }
    }
}