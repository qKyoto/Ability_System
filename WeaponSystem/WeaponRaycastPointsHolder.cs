using System.Collections.Generic;
using UnityEngine;

namespace SoulStitcher.Scripts.Game.WeaponSystem
{
    public class WeaponRaycastPointsHolder : MonoBehaviour
    {
        private const int MIN_SEGMENTS_AMOUNT = 1;
        
        [SerializeField] private Vector3 _startPointOffset;
        [SerializeField] private Vector3 _direction;
        [SerializeField, Min(0)] private float _lenght;
        [SerializeField, Min(MIN_SEGMENTS_AMOUNT)] private int _amountSegments;
#if UNITY_EDITOR
        [Header("Editor only")]
        [SerializeField] private float _pointRadius;
#endif

        public List<Vector3> GetRaycastPoints()
        {
            List<Vector3> points = new();
            
            Vector3 startPoint = transform.TransformPoint(transform.localPosition + _startPointOffset);
            Vector3 endPoint = startPoint + transform.TransformDirection(_direction * _lenght);
            
            points.Add(startPoint);
            points.Add(endPoint);
            
            for (int i = MIN_SEGMENTS_AMOUNT; i < _amountSegments; i++)
            {
                float distance = _lenght / _amountSegments * i;
                Vector3 subPoint = startPoint + transform.TransformDirection(_direction * distance);
                points.Add(subPoint);
            }

            return points;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Vector3 startPosition = transform.TransformPoint(transform.localPosition + _startPointOffset);
            Vector3 endPosition = startPosition + transform.TransformDirection(_direction * _lenght);
            
            Gizmos.color = Color.white;
            Gizmos.DrawRay(startPosition, transform.TransformDirection( _direction * _lenght));
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(startPosition, _pointRadius);
            Gizmos.DrawSphere(endPosition, _pointRadius);

            for (int i = MIN_SEGMENTS_AMOUNT; i < _amountSegments; i++)
            {
                float distance = _lenght / _amountSegments * i;
                Gizmos.DrawSphere(startPosition + transform.TransformDirection(_direction * distance), _pointRadius);
            }
        }
#endif
    }
}