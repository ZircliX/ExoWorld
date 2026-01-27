using System.Collections.Generic;
using Helteix.Singletons.MonoSingletons;

namespace OverBang.ExoWorld.Gameplay
{
    public class PlayerGadgetInventory : MonoSingleton<PlayerGadgetInventory>
    {
        private Dictionary<IGadget, int> gadgets;

        protected override void OnAwake() 
        {
            gadgets =  new Dictionary<IGadget, int>();
        }

        public void AddGadget(IGadget gadgetType, int amount)
        {
            if (!gadgets.TryAdd(gadgetType, amount)) gadgets[gadgetType] += amount;
        }
    
        public bool GetGadgetCount(IGadget gadgetType, out int amount)
        {
            return gadgets.TryGetValue(gadgetType, out amount);
        }
    
        public bool UseGadget(IGadget gadgetType)
        {
            if (gadgets.ContainsKey(gadgetType) && gadgets[gadgetType] > 0)
            {
                gadgets[gadgetType]--;
                return true;
            }
            return false;
        }
    }
}