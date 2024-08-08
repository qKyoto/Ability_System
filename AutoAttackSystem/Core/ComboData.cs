using System;
using UnityEngine;

namespace SoulStitcher.Scripts.Game.AutoAttackSystem.Core
{
    [Serializable]
    public struct ComboData
    {
        [SerializeField] private string _animationName;
        [SerializeField] private float _freezeTime;

        public string AnimationName => _animationName;
        public float FreezeTime => _freezeTime;
    }
}