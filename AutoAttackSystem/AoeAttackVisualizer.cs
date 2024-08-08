using UnityEngine;

namespace SoulStitcher.Scripts.Game.AutoAttackSystem
{
    public class AoeAttackVisualizer : AttackVisualizer
    {
        private const float SPRITE_MULTIPLAYER = 2;
        
        [SerializeField] private SpriteRenderer _sprite;
        
        public void Initialize(float range)
        {
            _sprite.size = Vector2.one * range * SPRITE_MULTIPLAYER;
        }
    }
}