using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SoulStitcher.Scripts.Game.StatsSystem
{
    public class Stat
    {
        private const int ROUND_VALUE = 4;
        private const int MIN_STAT_VALUE = 0;
        
        private float _baseValue;
        private List<StatModifier> _statModifiers;

        public float BaseValue
        {
            get => _baseValue;
            set
            {
                _baseValue = value;
                _baseValue = Mathf.Clamp(_baseValue, MIN_STAT_VALUE, _baseValue);

                OnUpdateStat();
            }
        }
        public float Value { get; private set; }
        public StatsSheet StatsSheet { get; }
        public StatType Type { get; }
        
        public event Action StatUpdated;

        public Stat()
        {
            _statModifiers = new List<StatModifier>();
        }

        public Stat(StatsSheet statsSheet, StatType type) : this()
        {
            StatsSheet = statsSheet;
            Type = type;
        }
        
        public Stat(StatsSheet statsSheet, StatType type, float baseValue) : this()
        {
            StatsSheet = statsSheet;
            Type = type;
            BaseValue = baseValue;
        }
        
        public void AddModifier(StatModifier statModifier)
        {
            if (statModifier == null) return;
            
            _statModifiers.Add(statModifier);
            _statModifiers = _statModifiers.OrderBy(modifier => modifier.Order).ToList();
            
            OnUpdateStat();
        }

        public bool RemoveModifier(StatModifier statModifier)
        {
            if (!_statModifiers.Remove(statModifier)) 
                return false;
            
            OnUpdateStat();
            return true;
        }

        public bool RemoveAllModifierFromSource(object source)
        {
            bool isRemoved = false;
            
            for (int i = _statModifiers.Count; i >= 0; i--)
            {
                if (_statModifiers[i].Source != source) continue;
                _statModifiers.RemoveAt(i);
                isRemoved = true;
            }
            
            if (isRemoved)
                OnUpdateStat();
            
            return isRemoved;
        }

        public void RegisterOnValueUpdatedHandler(Action action)
        {
            StatUpdated += action;
        }

        public void UnregisterOnValueUpdateHandler(Action action)
        {
            StatUpdated -= action;
        }
        
        public void Validate()
        {
            OnUpdateStat();
        }

        private void OnUpdateStat()
        {
            CalculateStatValue();
            StatUpdated?.Invoke();
        }

        private void CalculateStatValue()
        {
            float derivedValue = StatFormulas.FORMULA_MAP[Type](StatsSheet, BaseValue);
            float flatSum =  GetFlatModifierValue();
            float percentSum = 1 + GetPercentSumModifierValue();
            float percentMultiply = GetPercentMultiplyModifierValue();

            float finalValue = (derivedValue + flatSum) * percentSum * percentMultiply;
            finalValue = Mathf.Clamp(finalValue, 0, finalValue);
            
            Value = (float)Math.Round(finalValue, ROUND_VALUE);
            Value = Mathf.Clamp(Value, 0, Value);
        }

        private float GetFlatModifierValue()
        {
            return _statModifiers.Where(modifier => modifier.ModifierType == StatModifierType.Flat).Sum(modifier => modifier.Value);
        }

        private float GetPercentSumModifierValue()
        {
            return _statModifiers.Where(modifier => modifier.ModifierType == StatModifierType.PercentSum).Sum(modifier => modifier.Value);
        }

        private float GetPercentMultiplyModifierValue()
        {
            return _statModifiers.Where(statModifier => statModifier.ModifierType == StatModifierType.PercentMultiply).Aggregate<StatModifier, float>(1, (current, statModifier) => current * (1f + statModifier.Value));
        }
        
#if UNITY_EDITOR
        public void SetStatValue(float value)
        {
            _baseValue = value;
            Value = value;
            
            OnUpdateStat();
        }
#endif
    }
}
