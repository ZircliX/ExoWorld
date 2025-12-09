using UnityEngine;

namespace OverBang.GameName.Core
{
    public static class PlayerInventory
    {
        public static int Trinitite = 1000;

        public static void ReceiveTrinitite(int amount)
        {
            Trinitite += amount;
        }
        
        public static void DecrementTrinitite(int amount)
        {
            Trinitite -= amount;
        }
        
        
        
    }
}