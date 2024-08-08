using System;
using SoulStitcher.Scripts.Game.AbilitySystem;
using SoulStitcher.Scripts.Game.DamageSystem;
using SoulStitcher.Scripts.Game.StatsSystem;
using SoulStitcher.Scripts.Game.Units;
using SoulStitcher.Scripts.Game.Units.Animations;
using SoulStitcher.Scripts.Game.Units.Modifiers;
using SoulStitcher.Scripts.Utilities.RuntimeUtility;
using UniRx;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;
using Unit = SoulStitcher.Scripts.Game.Units.Unit;

namespace SoulStitcher.Scripts.Game.AutoAttackSystem.Core
{
    [Serializable]
    public abstract class AutoAttack : IAutoAttack
    {
        private const int MIN_COMBO_AMOUNT = 1;
        private const int MAX_CRITICAL_DAMAGE_PERCENT = 101;
        
        [Space(10)]
        [SerializeField] private AnimationsControllerBase _animationsController;
        [SerializeField] private AttackEventReceiver _eventReceiver;
        [SerializeField] private Unit _unit;
        [Space(10)]
        [SerializeField] private GenericDictionary<ModifiersOption, UnitModifierFlags> _permissionsOnPreStart;
        [SerializeField] private GenericDictionary<ModifiersOption, UnitModifierFlags> _permissionsOnStart;
        [SerializeField] private GenericDictionary<ModifiersOption, UnitModifierFlags> _permissionsOnEnd;
        [Space(10)]
        [SerializeField] private ComboData[] _comboData;

        private IDisposable _comboTimer;
        private int _comboIndex;
        
        protected DamageManager DamageManager { get; private set; }
        protected AttackEventReceiver EventReceiver => _eventReceiver;
        protected IUnit Unit => _unit;

        public bool CanAttack => !_unit.UnitFlags.HasFlag(UnitModifierFlags.BlockAutoAttack);
        public bool HasCombo => _comboData.Length > MIN_COMBO_AMOUNT;
        
        protected event Action AttackStarted;
        
        public virtual void Initialize(DiContainer diContainer)
        {
            DamageManager = diContainer.Resolve<DamageManager>();
            
            _eventReceiver.AnimationStarted += OnAnimationStarted;
            _eventReceiver.AnimationEnded += OnAnimationEnded;
        }

        public virtual void Dispose()
        {
            _comboTimer?.Dispose();
            
            _eventReceiver.AnimationStarted -= OnAnimationStarted;
            _eventReceiver.AnimationEnded -= OnAnimationEnded;
        }

        public void PrePerform()
        {
            if (!CanAttack)
                return;
            
            OnPrePerformed();
            UpdatePermissions(_permissionsOnPreStart);
        }

        public void Perform()
        {
            if (!CanAttack)
                return;

            OnPerformed();
            UpdatePermissions(_permissionsOnStart);
            _animationsController.PlayAnimation(Animator.StringToHash(_comboData[_comboIndex].AnimationName));
            AttackStarted?.Invoke();
            StartComboTimer();
        }
        
        public virtual void DrawGizmos() { }

        protected virtual void OnPrePerformed() { }

        protected virtual void OnPerformed() { }
        
        protected (int, bool) CalculateDamage()
        {
            float damageRange = Random.Range(-_unit.StatsSheet.GetStat(StatType.DamageRange).Value, _unit.StatsSheet.GetStat(StatType.DamageRange).Value);
            int damageValue = (int)(_unit.StatsSheet.GetStat(StatType.Damage).Value + damageRange); 
            int criticalChance =  Random.Range(0, MAX_CRITICAL_DAMAGE_PERCENT);
            (int damage, bool isCritical) calculatedDamage = (damageValue, false);

            if (criticalChance <= _unit.StatsSheet.GetStat(StatType.CriticalDamageChance).Value)
            {
                calculatedDamage.damage = (int)(calculatedDamage.damage * _unit.StatsSheet.GetStat(StatType.CriticalDamageMultiplier).Value);
                calculatedDamage.isCritical = true;
            }
            
            return calculatedDamage;
        }
        
        private void StartComboTimer()
        {
            if (_comboTimer != null || !HasCombo)
                return;
            
            _comboTimer = Observable.Timer(TimeSpan.FromSeconds(_comboData[_comboIndex].FreezeTime))
                .Subscribe(_ =>
                {
                    _comboIndex++;
                    
                    if (_comboIndex >= _comboData.Length)
                        _comboIndex = 0;
                    
                    _unit.RemoveFlags(UnitModifierFlags.BlockAutoAttack);
                    _comboTimer = null;
                });
        }

        private void OnAnimationStarted() { }

        private void OnAnimationEnded()
        {
            _comboIndex = 0;
            UpdatePermissions(_permissionsOnEnd);
        }
        
        private void UpdatePermissions(GenericDictionary<ModifiersOption, UnitModifierFlags> permissions)
        {
            foreach ((ModifiersOption option, UnitModifierFlags permission) in permissions)
            {
                switch (option)
                {
                    case ModifiersOption.Add:
                        _unit.AddFlags(permission);
                        break;
                    case ModifiersOption.Remove:
                        _unit.RemoveFlags(permission);
                        break;
                }
            }
        }
    }
}