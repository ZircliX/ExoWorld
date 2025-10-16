using System.Collections.Generic;

namespace OverBang.Pooling.Dependencies
{
    public interface IPoolDependencyProvider
    {
        void FillDependencies(List<IPoolConfig> poolConfigs);
    }
}