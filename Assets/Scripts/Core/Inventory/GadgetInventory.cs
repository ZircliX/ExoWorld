using System;
using System.Collections.Generic;
using OverBang.ExoWorld.Core.Abilities.Gadgets;
using UnityEngine;

namespace OverBang.ExoWorld.Core.Inventory
{
    public class GadgetInventory
    {
        private Dictionary<GadgetData, List<IGadget>> gadgets = new();
        public HashSet<GadgetData> GadgetDatas { get; private set; } = new HashSet<GadgetData>();

        public GadgetInventory()
        {
            GadgetData[] gadgetDatas = Resources.LoadAll<GadgetData>("Gadgets");
            for (int i = 0; i < gadgetDatas.Length; i++)
            {
                gadgets.Add(gadgetDatas[i], new List<IGadget>());
                GadgetDatas.Add(gadgetDatas[i]);
            }
        }

        public void AddGadget(GadgetData data, int amount, Func<IGadget> getGadget)
        {
            if (gadgets.TryGetValue(data, out List<IGadget> list))
            {
                for (int i = 0; i < amount; i++)
                {
                    list.Add(getGadget());
                }
            }
        }

        public void RemoveGadget(GadgetData data, int amount)
        {
            if (gadgets.TryGetValue(data, out List<IGadget> list))
            {
                for (int i = 0; i < amount; i++)
                {
                    if (list.Count > 0)
                    {
                        list.RemoveAt(0);
                    }
                }
            }
        }
    
        public bool GetGadgetCount(GadgetData data, out int amount)
        {
            if (gadgets.TryGetValue(data, out List<IGadget> list))
            {
                amount = list.Count;
                return true;
            }
            
            amount = 0;
            return false;
        }
    
        public bool TryGetGadget(GadgetData data, out IGadget gadget)
        {
            if (gadgets.TryGetValue(data, out List<IGadget> list))
            {
                gadget = list[0];
                return true;
            }
            
            gadget = null;
            return false;
        }
    }
}