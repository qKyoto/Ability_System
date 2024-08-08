using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoulStitcher.Scripts.Game.StatsSystem
{
    [Serializable]
    public class StatsSheet : ISerializationCallbackReceiver
    {
        [SerializeField] private StatSerialization[] _statsData;
        
        private readonly Dictionary<StatType, Stat> _stats;
        private readonly Dictionary<AttributeType, Attribute> _attributes;

        public StatsSheet()
        {
            _stats = new Dictionary<StatType, Stat>();
            _attributes = new Dictionary<AttributeType, Attribute>();

            RegisterStats();
            RegisterAttributes();
            RegisterStatFormulaDependencies();
        }

        public Stat GetStat(StatType type)
        {
            return _stats[type];
        }

        public Attribute GetAttribute(AttributeType type)
        {
            return _attributes[type];
        }
        
        private void RegisterStats()
        {
            foreach (StatType statType in StatFormulas.STAT_TYPES)
                _stats.Add(statType, new Stat(this, statType));
        }

        private void RegisterAttributes()
        {
            foreach (AttributeType attributeType in AttributeFormulas.ATTRIBUTE_TYPES)
                _attributes.Add(attributeType, new Attribute(this, attributeType));
        }
        
        private void RegisterStatFormulaDependencies()
        {
            foreach (StatType type in _stats.Keys)
            {
                Stat stat = _stats[type];
                StatType[] dependencies = StatFormulas.DEPENDENCY_MAP[type];
                
                if(dependencies == null) 
                    continue;

                foreach (StatType dependency in dependencies)
                    _stats[dependency].RegisterOnValueUpdatedHandler(stat.Validate);
            }
        }

        public void OnBeforeSerialize()
        {
            List<StatSerialization> statsData = new();
            foreach ((StatType key, Stat value) in _stats)
                statsData.Add(new StatSerialization(key, value.BaseValue));
            _statsData = statsData.ToArray();
        }   

        public void OnAfterDeserialize()
        {
            foreach (StatSerialization statData in _statsData)
                _stats[statData.StatType].BaseValue = statData.BaseValue;
        }
        
        [Serializable]
        private struct StatSerialization
        {
            [SerializeField] private StatType _statType;
            [SerializeField] private float _baseValue;

            public StatType StatType => _statType;
            public float BaseValue => _baseValue;

            public StatSerialization(StatType statType, float baseValue)
            {
                _statType = statType;
                _baseValue = baseValue;
            }
        }
    }
}