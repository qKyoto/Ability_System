using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using SoulStitcher.Scripts.Game.AbilitySystem.Actions.Core;
using SoulStitcher.Scripts.Game.DamageSystem;
using SoulStitcher.Scripts.Game.Extensions.Coroutine;
using UniRx;
using UnityEngine;
using Zenject;

namespace SoulStitcher.Scripts.Game.AbilitySystem
{
    public enum AbilityStateType
    {
        Ready,
        PreCast,
        Casting,
        Cooldown
    }

    [UsedImplicitly]
    public class Ability
    {
        private readonly AbilityConfig _config;
        private readonly AbilityCaster _caster;
        private readonly List<Coroutine> _runningRoutines;
        
        private AbilityStateType _state = AbilityStateType.Ready;
        private IDisposable _cooldownTimer;
        private IDisposable _tickObservable;
        private Coroutine _castRoutine;
        private float _normalizedCooldownValue;
        private int _charges;
        
        public AbilityStateType State
        {
            get => _state;
            private set
            {
                if (_state == value)
                    return;
                
                _state = value;
                AbilityStateChanged?.Invoke();
            }
        }

        public float NormalizedCooldownValue
        {
            get => _normalizedCooldownValue;
            private set
            {
                _normalizedCooldownValue = value;
                CooldownUpdated?.Invoke();
            }
        }

        public event Action AbilityStateChanged;
        public event Action Performed;
        public event Action CooldownUpdated;

        public Ability(AbilityCaster caster, AbilityConfig config, DiContainer container)
        {
            _config = config;
            _caster = caster;
            _charges = _config.Hits;
            _runningRoutines = new List<Coroutine>();
            
            InitializeActions(container);
        }

        public void PreCast(AbilityContext context)
        {
            State = AbilityStateType.PreCast;
            foreach (IAbilityAction preCastAction in _config.PreCastActions)
            {
                Coroutine coroutine = preCastAction.Execute(context).Start();
                _runningRoutines.Add(coroutine);
            }
        }

        public void Cast(AbilityContext context)
        {
            Cooldown();
            
            State = AbilityStateType.Casting;
            _castRoutine = CastRoutine().Start();

            IEnumerator CastRoutine()
            {
                foreach (IAbilityAction action in _config.CastActions)
                {
                    Coroutine coroutine = action.Execute(context).Start();
                    _runningRoutines.Add(coroutine);
                    yield return coroutine;
                }
                
                ClearRunningRoutines();
                Performed?.Invoke();
            }
        }

        public void Cancel()
        {
            _castRoutine.Stop();

            ClearRunningRoutines();
            CancelActions();
        }

        private void ClearRunningRoutines()
        {
            for (int i = _runningRoutines.Count-1; i > 0; i--)
            {
                _runningRoutines[i].Stop();
                _runningRoutines.RemoveAt(i);
            }
        }

        private void InitializeActions(DiContainer container)
        {
            foreach (IAbilityAction action in _config.PreCastActions)
                action.Initialize(container);
            foreach (IAbilityAction action in _config.CastActions)
                action.Initialize(container);
        }

        private void CancelActions()
        {
            foreach (IAbilityAction action in _config.PreCastActions)
                action.Cancel();
            foreach (IAbilityAction action in _config.CastActions)
                action.Cancel();
        }

        private void Cooldown()
        {
            State = AbilityStateType.Cooldown;
            NormalizedCooldownValue = 0;

            if (_config.Cooldown == CooldownType.Time)
                CooldownByTime();
            else
                CooldownByAutoAttackHit();
        }

        private void CooldownByTime()
        {
            float cooldownTime = 0;

            _cooldownTimer = Observable.EveryUpdate()
                .TakeWhile(_ => cooldownTime < _config.CooldownTime)
                .Finally(() =>
                {
                    State = AbilityStateType.Ready;
                })
                .Subscribe(_ =>
                {
                    cooldownTime += Time.deltaTime;
                    NormalizedCooldownValue = cooldownTime / _config.CooldownTime;
                });
        }

        private void CooldownByAutoAttackHit()
        {
            _charges = 0;
            _caster.Unit.DamageProcessed += OnDamageProcessed;

            void OnDamageProcessed(int damage, DamageSourceType damageSourceType, TeamTag receiverTag)
            {
                if (damageSourceType != DamageSourceType.AutoAttack ||
                    receiverTag == TeamTag.DistructableObject)
                    return;
                
                _charges++;
                NormalizedCooldownValue = (float)_charges / _config.Hits;

                if (_charges != _config.Hits) 
                    return;
                
                _state = AbilityStateType.Ready;
                _caster.Unit.DamageProcessed -= OnDamageProcessed;
            }
        }
    }
}