using UnityEngine;

namespace SoulStitcher.Scripts.Game.AutoAttackSystem
{
    public class DirectionalAttackVisualizer : AttackVisualizer
    {
        [SerializeField] private LayerMask _obstacleMask;
        [SerializeField] private Vector3 _halfExtents;
        [SerializeField] private SpriteRenderer _sprite;
        [SerializeField] private float _minDistance;
        [SerializeField] private float _maxDistance;
        
        private void Update()
        {
            if (Physics.BoxCast(transform.position, _halfExtents, transform.forward, out RaycastHit hit, transform.rotation, _maxDistance, _obstacleMask))
            {
                float distance = hit.distance < _minDistance ? _minDistance : hit.distance;
                _sprite.size = new Vector2(_sprite.size.x, distance);
            }
            else
            {
                _sprite.size = new Vector2(_sprite.size.x, _maxDistance);
            }
        }
    }
}