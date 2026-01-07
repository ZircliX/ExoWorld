using UnityEngine;

namespace OverBang.GameName.Core
{
    public static class PlayerInventory
    {
        public static int Trinitite = 100000;

        public static void ReceiveTrinitite(int amount)
        {
            Trinitite += amount;
        }
        
        public static int DecrementTrinitite(int amount)
        {
            return Trinitite -= amount;
        }
        
        
        
    }
}