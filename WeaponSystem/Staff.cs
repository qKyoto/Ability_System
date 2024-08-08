using UnityEngine;

namespace SoulStitcher.Scripts.Game.WeaponSystem
{
    public class Staff : Weapon
    {
        [SerializeField] private Vector3 _projectileCastOffset;
#if UNITY_EDITOR
        [Header("Editor only")]
        [SerializeField] private float _projectilePointRadius;
#endif
        public Vector3 ProjectileCastPosition => transform.TransformPoint(transform.localPosition + _projectileCastOffset); 
        
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Vector3 projectilePosition = transform.TransformPoint(transform.localPosition + _projectileCastOffset);
            Gizmos.DrawSphere(projectilePosition, _projectilePointRadius);            
        }
#endif
    }
}