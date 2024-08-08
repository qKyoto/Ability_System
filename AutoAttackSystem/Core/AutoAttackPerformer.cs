using SoulStitcher.Scripts.Utilities.RuntimeUtility.SubClassSelector;
using UnityEngine;
using Zenject;

namespace SoulStitcher.Scripts.Game.AutoAttackSystem.Core
{
    public abstract class AutoAttackPerformer : MonoBehaviour
    {
        [SerializeReference, SubclassSelector(typeof(IAutoAttack))] private IAutoAttack _autoAttack;

        private DiContainer _diContainer;
        
        protected IAutoAttack AutoAttack => _autoAttack;

        public bool CanAttack => _autoAttack.CanAttack;
        
        [Inject]
        private void Construct(DiContainer diContainer) => _diContainer = diContainer;

        private void Awake() => _autoAttack.Initialize(_diContainer);

        private void OnDestroy() => _autoAttack.Dispose();

        public void PrePerformAttack() => _autoAttack.PrePerform();

        public void PerformAttack() => _autoAttack.Perform();

#if UNITY_EDITOR
        private void OnDrawGizmos() => _autoAttack?.DrawGizmos();
#endif
    }
}