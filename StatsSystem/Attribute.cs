using System;
using UnityEngine;

namespace SoulStitcher.Scripts.Game.StatsSystem
{
    public class Attribute
    {
        private const float MIN_VALUE = 0;

        private readonly Stat _stat;
        
        private float _value;
        private float _maxValue;
        
        public float NormalizedValue { get; private set; }
        public AttributeType AttributeType { get; }
        public float Value
        {
            get => _value;
            set
            {
                _value = value;
                _value = Mathf.Clamp(_value, MIN_VALUE, MaxValue);
                
                if (_maxValue != 0)
                    NormalizedValue = _value / _maxValue;

                AttributeUpdated.Invoke();
            }
        }

        public float MinValue => MIN_VALUE;
        public float MaxValue => _maxValue;

        public event Action AttributeUpdated = delegate {  };

        public Attribute(StatsSheet sheet, AttributeType attributeType)
        {
            AttributeType = attributeType;
            _stat = sheet.GetStat(AttributeFormulas.DEPENDENCY_MAP[attributeType]);
            NormalizedValue = AttributeFormulas.START_ATTRIBUTE_VALUE[_stat.Type];
            _stat.StatUpdated += OnUpdateStat;
            _maxValue = _stat.Value;
            _value = _maxValue;
        }

        ~Attribute() 
        {
            _stat.StatUpdated -= OnUpdateStat;
        }
        
        private void OnUpdateStat()
        {
            _maxValue = _stat.Value;
            Value = NormalizedValue * _maxValue;
        }
    }
}