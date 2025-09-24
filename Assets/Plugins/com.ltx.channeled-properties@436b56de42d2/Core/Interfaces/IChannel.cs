namespace LTX.ChanneledProperties
{
    public interface IChannel<T>
    {
        public T Value { get; internal set; }
    }

}