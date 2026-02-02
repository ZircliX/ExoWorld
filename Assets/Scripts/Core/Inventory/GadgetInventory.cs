using System.Collections.Generic;
using OverBang.ExoWorld.Core.Abilities.Gadgets;
using UnityEngine;

namespace OverBang.ExoWorld.Core.Inventory
{
    public class GadgetInventory
    {
        private class RuntimeGadgetInfo
        {
            public IGadget Gadget;
            public int Amount;
            
            public bool IsUsable => Gadget != null &&  Amount > 0;
        }
        
        private Dictionary<GadgetData, RuntimeGadgetInfo> gadgets = new Dictionary<GadgetData, RuntimeGadgetInfo>();
        public HashSet<GadgetData> GadgetDatas { get; private set; } = new HashSet<GadgetData>();

        public GadgetInventory()
        {
            GadgetData[] gadgetDatas = Resources.LoadAll<GadgetData>("Gadgets");
            for (int i = 0; i < gadgetDatas.Length; i++)
            {
                Debug.Log($"Gadget data {i}: {gadgetDatas[i]}");
                gadgets.Add(gadgetDatas[i], new RuntimeGadgetInfo());
                GadgetDatas.Add(gadgetDatas[i]);
            }
        }

        public void AddGadget(GadgetData data, IGadget gadgetType, int amount)
        {
            if (gadgets.TryGetValue(data, out RuntimeGadgetInfo gadgetInfo))
            {
                gadgetInfo.Gadget = gadgetType;
                gadgetInfo.Amount += amount;
            }
        }
    
        public bool GetGadgetCount(GadgetData data, out int amount)
        {
            if (gadgets.TryGetValue(data, out RuntimeGadgetInfo gadgetInfo))
            {
                amount = gadgetInfo.Amount;
                return true;
            }
            
            amount = 0;
            return false;
        }
    
        public bool TryGetGadget(GadgetData data, out IGadget gadget)
        {
            if (gadgets.TryGetValue(data, out RuntimeGadgetInfo gadgetInfo) && gadgetInfo.IsUsable)
            {
                gadget = gadgetInfo.Gadget;
                gadgetInfo.Amount--;
                return true;
            }
            
            gadget = null;
            return false;
        }
    }
}