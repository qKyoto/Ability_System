using UnityEngine;

namespace SoulStitcher.Scripts.Game.AbilitySystem
{
    public class EnemyAbilityCaster : AbilityCaster
    {
        public const string ENEMY_ABILITY_CASTER = "enemyAbilityCaster";

        private Transform _player;
        private bool _playerDetected;

        public void SetPlayerReference(Transform player)
        {
            _player = player;
            _playerDetected = true;
        }

        protected override Vector3 GetTargetPoint(AbilityConfig config)
        {
            return config.Direction switch
            {
                AbilityDirectionType.Point => _playerDetected ? _player.position : transform.position,
                AbilityDirectionType.Aoe => _playerDetected ? _player.position : transform.position,
                _ => _playerDetected ? _player.position : transform.position
            };
        }
    }
}