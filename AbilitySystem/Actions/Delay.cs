using System;
using System.Collections;
using SoulStitcher.Scripts.Game.AbilitySystem.Actions.Core;
using UnityEngine;

namespace SoulStitcher.Scripts.Game.AbilitySystem.Actions
{
    [Serializable]
    public class Delay : AbilityAction
    {
        [SerializeField, Min(0)] private float _delay;

        public override IEnumerator Execute(AbilityContext context)
        {
            yield return new WaitForSeconds(_delay);
        }
    }
}