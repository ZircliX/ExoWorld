using System;

namespace OverBang.Pooling.Resource
{
    public interface IPoolAsset
    {
        void Load(Action<UnityEngine.Object> onLoad);
        void Unload();
    }
}