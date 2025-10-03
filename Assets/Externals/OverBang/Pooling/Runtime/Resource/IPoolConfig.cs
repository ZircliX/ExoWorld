using OverBang.Pooling.Resource;

namespace OverBang.Pooling
{
    public interface IPoolConfig
    {
        int PoolSize { get; }
        PoolResource PoolResource { get; }
    }
}