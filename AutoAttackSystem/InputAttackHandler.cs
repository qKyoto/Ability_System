using System;
using SoulStitcher.Scripts.Game.AbilitySystem;
using SoulStitcher.Scripts.Game.Units.Modifiers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SoulStitcher.Scripts.Game.AutoAttackSystem
{
    public class InputAttackHandler : MonoBehaviour
    {
        [SerializeField] private PlayerAutoAttackPerformer _attackPerformer;
        [SerializeField] private PlayerAbilityCaster _abilityCaster;
        [Space(10)]
        [SerializeField] private InputActionReference _autoAttackInputAction;
        [SerializeField] private InputActionReference[] _abilityInputActions;
        [Space(10)]
        [SerializeField] private AbilityConfig[] _abilityConfigs;

        private Action<InputAction.CallbackContext>[] _abilityActions;
        private Action<InputAction.CallbackContext> _autoAttackAction;

        private void Start()
        {
            _abilityActions = new Action<InputAction.CallbackContext>[_abilityInputActions.Length];

            _autoAttackAction = _ => OnAutoAttackRequested(_autoAttackInputAction.action.IsPressed());
            _autoAttackInputAction.action.performed += _autoAttackAction;
            _autoAttackInputAction.action.canceled += _autoAttackAction;

            for (int i = 0; i < _abilityInputActions.Length; i++)
            {
                int abilityIndex = i;
                _abilityActions[i] = _ => OnAbilityRequested(abilityIndex, _abilityInputActions[abilityIndex].action.IsPressed());
                _abilityInputActions[i].action.performed += _abilityActions[i];
                _abilityInputActions[i].action.canceled += _abilityActions[i];
            }
        }

        private void OnDestroy()
        {
            _autoAttackInputAction.action.performed -= _autoAttackAction;
            _autoAttackInputAction.action.canceled -= _autoAttackAction;
            
            for (int i = 0; i < _abilityInputActions.Length; i++)
            {
                _abilityInputActions[i].action.performed -= _abilityActions[i];
                _abilityInputActions[i].action.canceled -= _abilityActions[i];
            }
        }

        public void ProcessInput(bool isEnable)
        {
            if (isEnable)
                Subscribe();
            else
                Unsubscribe();
        }

        private void Subscribe()
        {
            
        }

        private void Unsubscribe()
        {
            
        }
        
        private void OnAutoAttackRequested(bool isPressed)
        {
            if (_abilityCaster.IsCasting && !_attackPerformer.CanAttack)
            {
                _autoAttackInputAction.action.Reset();
                return;
            }
            
            switch (isPressed)
            {
                case true when _attackPerformer.Activation == InputActivationType.OnRelease:
                    _attackPerformer.PrePerformAttack();
                    break;
                case true when _attackPerformer.Activation == InputActivationType.OnPressing:
                case false when _attackPerformer.Activation == InputActivationType.OnRelease:
                    _attackPerformer.PerformAttack();
                    break;
            }
        }

        private void OnAbilityRequested(int abilityIndex, bool isPressed)
        {
            if (!_abilityCaster.CanCast(_abilityConfigs[abilityIndex]))
            {
                _abilityInputActions[abilityIndex].action.Reset();
                return;
            }
            
            switch (isPressed)
            {
                case true when _abilityConfigs[abilityIndex].Triggering == InputActivationType.OnRelease:
                    _abilityCaster.PreCast(_abilityConfigs[abilityIndex]);
                    break;
                case true when _abilityConfigs[abilityIndex].Triggering == InputActivationType.OnPressing:
                case false when _abilityConfigs[abilityIndex].Triggering == InputActivationType.OnRelease:
                    _abilityCaster.Cast(_abilityConfigs[abilityIndex]);
                    break;
            }
        }
    }
}