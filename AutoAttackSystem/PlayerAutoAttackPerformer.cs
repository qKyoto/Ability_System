using SoulStitcher.Scripts.Game.AbilitySystem;
using SoulStitcher.Scripts.Game.AutoAttackSystem.Core;
using UnityEngine;

namespace SoulStitcher.Scripts.Game.AutoAttackSystem
{
    public class PlayerAutoAttackPerformer : AutoAttackPerformer
    {
        [SerializeField] private InputActivationType _activation;
        
        public InputActivationType Activation => _activation;
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (AutoAttack is { HasCombo: true })
                _activation = InputActivationType.OnPressing;
        }
#endif
    }
}