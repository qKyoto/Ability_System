using System;
using Zenject;

namespace SoulStitcher.Scripts.Game.AutoAttackSystem.Core
{
    public interface IAutoAttack : IDisposable
    {
        public bool CanAttack { get; }
        public bool HasCombo { get; }
        public void Initialize(DiContainer diContainer);
        public void PrePerform();
        public void Perform();
        public void DrawGizmos();
    }
}