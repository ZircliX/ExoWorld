using OverBang.GameName.Core;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    [CreateStrategyFor(typeof(DfoStrategyData))]
    public class DfoAbilityStrategy : IAbilityStrategy<DfoData, DfoStrategyData>
    {
        private DfoStrategyData strategyData;
        private DfoData data;
        
        private MissileManager missileManager;
        private IAbilityCaster caster;
        
        private float currentActivationTime;
        private IAbility<DfoData> ability;
        
        public void Initialize(IAbility<DfoData> ability, IAbilityCaster caster, DfoStrategyData data)
        {
            this.caster = caster;
            this.data = ability.Data;
            this.ability = ability;
            strategyData = data;
        }
        
        public void Begin(IAbility<DfoData> ability)
        {
            //Debug.Log("Dfo BEGIN");
            
            currentActivationTime = strategyData.ActivationTime;
            
            DfoBalise balise = Object.Instantiate(ability.Data.DfoBalisePrefab, caster.transform.position + caster.Forward * 3.5f, Quaternion.identity);
            balise.Initialize(data, caster.Forward);
            
            missileManager = new MissileManager(data, strategyData, balise.transform);
            missileManager.OnEndMissiles += OnEndMissiles;
        }

        public void Tick(IAbility<DfoData> ability, float deltaTime)
        {
            //Debug.Log($"ActivationTime remaining: {currentActivationTime}");
            if (currentActivationTime > 0)
            {
                currentActivationTime -= deltaTime;
                return;
            }
            
            missileManager.Tick(deltaTime);
        }

        public void End(IAbility<DfoData> ability)
        {
            missileManager.OnEndMissiles -= OnEndMissiles;
        }
        
        public void Dispose(IAbility<DfoData> ability)
        {
        }

        private void OnEndMissiles()
        {
            ability.End();
        }
    }
}