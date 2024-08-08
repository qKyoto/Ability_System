using SoulStitcher.Scripts.Game.StatsSystem;
using SoulStitcher.Scripts.Game.Units;
using SoulStitcher.Scripts.Game.Units.Interaction;

namespace SoulStitcher.Scripts.Game.AbilitySystem.StatusEffects
{
    public interface IEffectable
    {
        public StatsSheet StatsSheet { get; }
        public UnitInteractionPoints InteractionPoints { get; }

        public bool HasEffect(StatusEffectType type);
        public void ApplyEffect(StatusEffect effect);
        public void RemoveEffect(StatusEffectType type);
        public void ResetEffect(StatusEffect effect);
    }
}