namespace OverBang.ExoWorld.Core.Upgrade
{
    [System.Serializable]
    public enum ResourceType
    { 
        Trinitite,
        Uranium,
        Plutonium, Credits
    }

    [System.Serializable]
    public struct ResourceAmount
    {
        public ResourceType Type;
        public int Amount;
    }
}