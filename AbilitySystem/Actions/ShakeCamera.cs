using System;
using System.Collections;
using SoulStitcher.Scripts.Game.AbilitySystem.Actions.Core;
using Zenject;

namespace SoulStitcher.Scripts.Game.AbilitySystem.Actions
{
    [Serializable]
    public class ShakeCamera : AbilityAction
    {
        public override void Initialize(DiContainer container)
        {
            //resolve camera operator            
        }

        public override IEnumerator Execute(AbilityContext context)
        {
            yield break;
        }
    }
}