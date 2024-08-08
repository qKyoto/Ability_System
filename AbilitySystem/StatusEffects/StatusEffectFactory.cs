using System.Collections.Generic;
using JetBrains.Annotations;
using Zenject;
using Object = UnityEngine.Object;

namespace SoulStitcher.Scripts.Game.AbilitySystem.StatusEffects
{
    public interface IStatusEffectFactory
    {
        public StatusEffect GetEffect(StatusEffect effectData);
    }
    
    [UsedImplicitly]
    public class StatusEffectFactory : IStatusEffectFactory
    {
        private readonly Dictionary<StatusEffectType, Stack<StatusEffect>> _inactiveEffects;
        private readonly DiContainer _diContainer;
        
        public StatusEffectFactory(DiContainer diContainer)
        {
            _diContainer = diContainer;
            _inactiveEffects = new Dictionary<StatusEffectType, Stack<StatusEffect>>();
        }

        public StatusEffect GetEffect(StatusEffect effectData)
        {
            if (!_inactiveEffects.ContainsKey(effectData.StatusEffectType))
                _inactiveEffects.Add(effectData.StatusEffectType, new Stack<StatusEffect>());                
            if (_inactiveEffects[effectData.StatusEffectType].Count == 0)
                _inactiveEffects[effectData.StatusEffectType].Push(CreateNewInstance(effectData));

            StatusEffect effectInstance = _inactiveEffects[effectData.StatusEffectType].Pop();
            effectInstance.Cleaned += OnEffectCleaned;

            return effectInstance;

            void OnEffectCleaned()
            {
                effectInstance.Cleaned -= OnEffectCleaned;
                _inactiveEffects[effectInstance.StatusEffectType].Push(effectInstance);
            }
        }
        
        private StatusEffect CreateNewInstance(StatusEffect effectData)
        {
            StatusEffect effectInstance = Object.Instantiate(effectData);
            _diContainer.Inject(effectInstance);

            return effectInstance;
        }
    }
}