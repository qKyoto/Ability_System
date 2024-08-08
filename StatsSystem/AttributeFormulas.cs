using System;
using System.Collections.Generic;

namespace SoulStitcher.Scripts.Game.StatsSystem
{
    public static class AttributeFormulas
    {
        public static readonly AttributeType[] ATTRIBUTE_TYPES = (AttributeType[]) Enum.GetValues(typeof(AttributeType));

        public static readonly Dictionary<AttributeType, StatType> DEPENDENCY_MAP = new()
        {
            { AttributeType.Hp, StatType.MaxHp }
        };

        public static readonly Dictionary<StatType, float> START_ATTRIBUTE_VALUE = new()
        {
            { StatType.MaxHp, 1f }
        };
    }
}