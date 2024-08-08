using NaughtyAttributes;
using UnityEngine;

namespace SoulStitcher.Scripts.Game.StatsSystem
{
    public class StatsProvider : MonoBehaviour
    {
        [HorizontalLine]
        [SerializeField] private StatsSheet _statsSheet;
        
        public StatsSheet StatsSheet => _statsSheet;
    }
}