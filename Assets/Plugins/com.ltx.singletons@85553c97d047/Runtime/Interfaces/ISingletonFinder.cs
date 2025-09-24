namespace LTX.Singletons
{
    public interface ISingletonFinder<T> where T : ISingleton
    {
        public bool TryFindExistingInstance(out T instance);
    }
}