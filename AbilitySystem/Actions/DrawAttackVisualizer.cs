using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using SoulStitcher.Scripts.Game.AbilitySystem.Actions.Core;
using SoulStitcher.Scripts.Game.AutoAttackSystem;
using SoulStitcher.Scripts.Game.Units;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace SoulStitcher.Scripts.Game.AbilitySystem.Actions
{
    [Serializable]
    public class DrawAttackVisualizer : AbilityAction
    {
        [SerializeField] private VisualizerType _type;
        [SerializeField] private DurationType _duration;
        
        [ShowIf(nameof(_duration), DurationType.Time), AllowNesting]
        [SerializeField] private float _time;
        
        [MinValue(0), MaxValue(360), ShowIf(nameof(_type), VisualizerType.Directional), AllowNesting]
        [SerializeField] private float _angle;
        
        [MinValue(0), ShowIf(nameof(_type), VisualizerType.Directional), AllowNesting]
        [SerializeField] private int _count;
        
        [ShowIf(nameof(_type), VisualizerType.Aoe), AllowNesting]
        [SerializeField] private float _range;
        
        [ShowIf(nameof(_duration), DurationType.AwaitInput), AllowNesting]
        [SerializeField] private InputActionReference _inputAction;
        
        [ShowIf(nameof(_type), VisualizerType.Directional), AllowNesting]
        [SerializeField] private DirectionalAttackVisualizer _directionalVisualizer;
        
        [ShowIf(nameof(_type), VisualizerType.Aoe), AllowNesting]
        [SerializeField] private AoeAttackVisualizer _aoeVisualizer;
        
        private IAttackVisualizerFactory _visualizerFactory;
        private IDisposable _timer;
        private List<AttackVisualizer> _visualizers;
        
        public override void Initialize(DiContainer container)
        {
            _visualizerFactory = container.Resolve<IAttackVisualizerFactory>();
            _visualizers = new List<AttackVisualizer>();
        }

        public override void Cancel()
        {
            _timer?.Dispose();
            DisableVisualizers();
        }

        public override IEnumerator Execute(AbilityContext context)
        {
            ShowVisualizers(context);
            
            if (_duration == DurationType.Time)
            {
                _timer = Observable.Timer(TimeSpan.FromSeconds(_time))
                    .Subscribe(_ =>
                    {
                        DisableVisualizers();
                    });
            }
            else
            {
                while (_inputAction.action.IsPressed())
                    yield return null;
                
                DisableVisualizers();
            }
        }

        private void ShowVisualizers(AbilityContext context)
        {
            IUnit owner = context.Caster.Unit;
            
            if (_type == VisualizerType.Directional)
            {
                (float angleStep, float angle) calculatedAngle = CalculateAngleStep();
                
                for (int i = 0; i < _count; i++)
                {
                    Quaternion rotation = GetTargetRotation(calculatedAngle.angle, owner);
                    DirectionalAttackVisualizer visualizer = _visualizerFactory.GetDirectionalVisualizer(_directionalVisualizer, owner.Transform.position, rotation, owner.Transform);
                    calculatedAngle.angle += calculatedAngle.angleStep;
                    _visualizers.Add(visualizer);
                }
            }
            else
            {
                    AoeAttackVisualizer visualizer = _visualizerFactory.GetAoeVisualizer(_aoeVisualizer, context.TargetPoint, Quaternion.identity, _range);
                    _visualizers.Add(visualizer);
            }
        }

        private void DisableVisualizers()
        {
            for (int i = _visualizers.Count - 1; i >= 0; i--)
            {
                _visualizers[i].gameObject.SetActive(false);
                _visualizers.RemoveAt(i);
            }
        }
        
        private (float, float) CalculateAngleStep()
        {
            float angleStep = _angle / (_count - 1);
            float angle = -(_angle / 2);
            (float, float) result = (angleStep, angle);
            
            return result;
        }
        
        private Quaternion GetTargetRotation(float angle, IUnit unit)
        {
            return unit.Transform.rotation * Quaternion.AngleAxis(angle, Vector3.up);
        }
        
        private enum VisualizerType
        {
            Directional,
            Aoe
        }
        
        private enum DurationType
        {
            Time,
            AwaitInput
        }
    }
}