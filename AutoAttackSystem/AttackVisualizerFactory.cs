using System.Collections.Generic;
using JetBrains.Annotations;
using SoulStitcher.Scripts.Game.Services.Global.PoolingSystem;
using UniRx;
using UnityEngine;

namespace SoulStitcher.Scripts.Game.AutoAttackSystem
{
    public interface IAttackVisualizerFactory
    {
        public DirectionalAttackVisualizer GetDirectionalVisualizer(DirectionalAttackVisualizer visualizer, Vector3 position, Quaternion rotation, Transform parent = null);
        public AoeAttackVisualizer GetAoeVisualizer(AoeAttackVisualizer visualizer, Vector3 position, Quaternion rotation, float range, Transform parent = null);
    }
    
    [UsedImplicitly]
    public class AttackVisualizerFactory : IAttackVisualizerFactory
    {
        private readonly IPoolingService _poolingService;
        private readonly List<AttackVisualizer> _activeVisualizers;

        public AttackVisualizerFactory(IPoolingService poolingService)
        {
            _poolingService = poolingService;
            _activeVisualizers = new List<AttackVisualizer>();

            CheckVisualizersCondition();
        }

        public DirectionalAttackVisualizer GetDirectionalVisualizer(DirectionalAttackVisualizer visualizer, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            DirectionalAttackVisualizer visualizerInstance = _poolingService.Get(visualizer, position, rotation, parent);
            _activeVisualizers.Add(visualizerInstance);
            return visualizerInstance;
        }
        
        public AoeAttackVisualizer GetAoeVisualizer(AoeAttackVisualizer visualizer, Vector3 position, Quaternion rotation, float range, Transform parent = null)
        {
            AoeAttackVisualizer visualizerInstance = _poolingService.Get(visualizer, position, rotation, parent);
            visualizerInstance.Initialize(range);
            _activeVisualizers.Add(visualizerInstance);
            return visualizerInstance;
        }

        private void CheckVisualizersCondition()
        {
            Observable.EveryUpdate()
                .Subscribe(_ =>
                {
                    for (int i = _activeVisualizers.Count - 1; i >= 0; i--)
                    {
                        if (_activeVisualizers[i].gameObject.activeSelf)
                            continue;
                        
                        _poolingService.Return(_activeVisualizers[i]);
                        _activeVisualizers.RemoveAt(i);
                    }
                });
        }
    }
}