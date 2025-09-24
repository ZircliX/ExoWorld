namespace LTX.Singletons
{
    public interface ISingletonFactory<out T> where T : ISingleton
    {
        public T CreateSingleton();
    }
}