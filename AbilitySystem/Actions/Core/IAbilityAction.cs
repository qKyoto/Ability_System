using System.Collections;
using Zenject;

namespace SoulStitcher.Scripts.Game.AbilitySystem.Actions.Core
{
    public interface IAbilityAction
    {
        public void Initialize(DiContainer container);
        public void Cancel();
        public IEnumerator Execute(AbilityContext context);
    }

    public abstract class AbilityAction : IAbilityAction
    {
        public virtual void Initialize(DiContainer container) { }

        public virtual void Cancel() { }

        public abstract IEnumerator Execute(AbilityContext context);
    }
}