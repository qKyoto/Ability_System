using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using SoulStitcher.Scripts.Game.AbilitySystem.Actions.Core;
using SoulStitcher.Scripts.Game.AbstractFactories;
using SoulStitcher.Scripts.Game.Units;
using SoulStitcher.Scripts.Game.Units.Interaction;
using UniRx;
using UnityEngine;
using Zenject;

namespace SoulStitcher.Scripts.Game.AbilitySystem.Actions
{
    [Serializable]
    public class PlayVfx : AbilityAction
    {
        [SerializeField] private ParticleSystem _vfx;
        [SerializeField] private Target _target;
        [SerializeField, ShowIf(nameof(IsUnitTarget)), AllowNesting] private UnitInteractionPointType _interactionPointType;
        [SerializeField, HideIf(nameof(IsUnitTarget)), AllowNesting] private float _overrideSize;
        [SerializeField] private float _lifeTime;

        private List<ParticleSystem> _playableVfx;
        private IVfxFactory _vfxFactory;
        
        private bool IsUnitTarget => _target is Target.Caster or Target.Targets;

        public override void Initialize(DiContainer container)
        {
            _playableVfx = new List<ParticleSystem>();
            _vfxFactory = container.Resolve<IVfxFactory>();
        }

        public override IEnumerator Execute(AbilityContext context)
        {
            switch (_target)
            {
                case Target.Caster:
                    foreach (InteractionPoint point in context.Caster.Unit.InteractionPoints.GetPoints(_interactionPointType))
                        RunVfx(point.transform.position, point.Size, point.transform);
                    break;
                case Target.Targets:
                    foreach (IUnit unit in context.Targets)
                    foreach (InteractionPoint point in unit.InteractionPoints.GetPoints(_interactionPointType))
                        RunVfx(point.transform.position, point.Size, point.transform);
                    break;
                case Target.CastPoint:
                    RunVfx(context.CastPoint, _overrideSize);
                    break;
                case Target.TargetPoint:
                    RunVfx(context.TargetPoint, _overrideSize);
                    break;
            }
            
            Observable.Timer(TimeSpan.FromSeconds(_lifeTime))
                .Subscribe(_ =>
                {
                    StopVfx();
                });
            
            yield break;
        }

        private void RunVfx(Vector3 position, float size, Transform parent = null)
        {
            ParticleSystem vfxInstance = _vfxFactory.GetVfx(_vfx, position, Quaternion.identity, size, parent);
            vfxInstance.Play();
            _playableVfx.Add(vfxInstance);
        }

        private void StopVfx()
        {
            for (int i = _playableVfx.Count - 1; i >= 0; i--)
            {
                _playableVfx[i].Stop();
                _playableVfx.RemoveAt(i);
            }
        }
        
        private enum Target
        {
            Caster,
            Targets,
            CastPoint,
            TargetPoint
        }
    }
}