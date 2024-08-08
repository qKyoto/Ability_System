using UnityEngine;

namespace SoulStitcher.Scripts.Game.AbilitySystem
{
    public class PlayerAbilityCaster : AbilityCaster
    {
        public void PreCast(AbilityConfig config)
        {
            AbilityContext context = AbilityFactory.GetContext(this, config, GetTargetPoint(config));
            ActiveAbility = Abilities[config];
            Abilities[config].PreCast(context);
        }
        
        protected override Vector3 GetTargetPoint(AbilityConfig config)
        {
            if (config.Direction == AbilityDirectionType.Aoe)
                return transform.position + transform.forward * config.AoeDistanceFromCaste;
            
            return transform.position;
        }
    }
}