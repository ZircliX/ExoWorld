namespace LTX.Singletons
{
    public sealed class MonoSingletonHolder<T, TFactory, TFinder> : BaseSingleton<T, TFactory, TFinder>
        where T : MonoSingleton<T, TFactory, TFinder>
        where TFactory : ISingletonFactory<T>, new()
        where TFinder : ISingletonFinder<T>, new()
    {
        public override bool IsInstance => instance != null && instance.InstanceHolder == this;
        internal T InternalInstance => Instance;
        internal bool InternalHasInstance => HasInstance;


        public override void Dispose()
        {
            instance = null;
        }
    }
}