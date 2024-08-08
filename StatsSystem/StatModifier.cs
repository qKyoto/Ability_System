using System;
using UnityEngine;

namespace SoulStitcher.Scripts.Game.StatsSystem
{
    [Serializable]
    public class StatModifier
    {
        [SerializeField] private StatType _statType;
        [SerializeField] private StatModifierType _modifierType;
        [SerializeField] private float _value;
        
        public int Order => (int)ModifierType;
        public object Source { get; }

        public float Value
        {
            get => _value;
            set
            {
                _value = value;
                _value = Mathf.Clamp(_value, 0,_value);
            }
        }

        public StatModifierType ModifierType => _modifierType;
        public StatType StatType => _statType;

        public StatModifier(StatModifierType modifierType, float value, object source = null)
        {
            _modifierType = modifierType;
            Value = value;
            Source = source;
        }
        
        public StatModifier(StatModifierType modifierType, StatType statType, float value, object source = null)
        {
            _modifierType = modifierType;
            _statType = statType;
            Value = value;
            Source = source;
        }
    }
}