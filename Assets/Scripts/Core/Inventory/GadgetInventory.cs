using System.Collections.Generic;
using OverBang.ExoWorld.Core.Abilities.Gadgets;
using UnityEngine;

namespace OverBang.ExoWorld.Core.Inventory
{
    public class GadgetInventory
    {
        private Dictionary<GadgetData, Stack<IGadget>> gadgets = new Dictionary<GadgetData, Stack<IGadget>>();
        public HashSet<GadgetData> GadgetDatas { get; private set; } = new HashSet<GadgetData>();

        public GadgetInventory()
        {
            GadgetData[] gadgetDatas = Resources.LoadAll<GadgetData>("Gadgets");
            for (int i = 0; i < gadgetDatas.Length; i++)
            {
                gadgets.Add(gadgetDatas[i], new Stack<IGadget>());
                GadgetDatas.Add(gadgetDatas[i]);
            }
        }

        public void AddGadget(GadgetData data, IGadget gadgetType, int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                if (gadgets.TryGetValue(data, out Stack<IGadget> gadget))
                {
                    gadget.Push(gadgetType);
                }
            }
        }
    
        public bool GetGadgetCount(GadgetData data, out int amount)
        {
            if (gadgets.TryGetValue(data, out Stack<IGadget> stack))
            {
                amount = stack.Count;
                return true;
            }
            
            amount = 0;
            return false;
        }
    
        public bool TryGetGadget(GadgetData data, out IGadget gadget)
        {
            if (gadgets.TryGetValue(data, out Stack<IGadget> stack))
            {
                return stack.TryPop(out gadget);
            }
            gadget = null;
            return false;
        }
    }
}