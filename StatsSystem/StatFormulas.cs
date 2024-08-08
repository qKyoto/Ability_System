using System;
using System.Collections.Generic;

namespace SoulStitcher.Scripts.Game.StatsSystem
{
    public static class StatFormulas 
    {
        public delegate float Formula(StatsSheet sheet, float baseValue);
        private static readonly Formula IDENTITY = (sheet, value) => value; // just returns base value

        public static readonly StatType[] STAT_TYPES = (StatType[]) Enum.GetValues(typeof(StatType));

        // There is an assumption that this hardcoded dependency map has no mistakes and reflects correctly against the formula map.
        // null list means the Stat has no dependencies.
        public static readonly Dictionary<StatType, StatType[]> DEPENDENCY_MAP = new()
        {
            { StatType.MaxHp, null },
            { StatType.MoveSpeed, null },
            { StatType.Damage, null },
            { StatType.DamageRange, null },
            { StatType.CriticalDamageChance, null },
            { StatType.CriticalDamageMultiplier, null }
        };

        public static readonly Dictionary<StatType, Formula> FORMULA_MAP = new()
        {
            { StatType.MaxHp, IDENTITY },
            { StatType.MoveSpeed, IDENTITY },
            { StatType.Damage, IDENTITY },
            { StatType.DamageRange, IDENTITY },
            { StatType.CriticalDamageChance, IDENTITY },
            { StatType.CriticalDamageMultiplier, IDENTITY }
        };
    }
}