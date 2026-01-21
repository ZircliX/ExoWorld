namespace OverBang.ExoWorld.Core
{
    public static class PlayerInventory
    {
        public static int Trinitite;

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