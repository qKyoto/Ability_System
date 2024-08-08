using SoulStitcher.Scripts.Game.Units;

namespace SoulStitcher.Scripts.Game.AbilitySystem.StatusEffects
{
    public interface IStatusEffectsManager
    {
        public void ApplyStatusEffect(IEffectable unit, StatusEffect statusEffect);
        public void RemoveStatusEffect(IEffectable unit, StatusEffectType type);
    }
}