using System;

namespace ZTools.Logger.Core.Enums
{
    [System.Serializable]
    [Flags]
    public enum LogMode
    {
        Log = 1 << 0,
        LogWarning = 1 << 1,
        LogError = 1 << 2,
    }
}