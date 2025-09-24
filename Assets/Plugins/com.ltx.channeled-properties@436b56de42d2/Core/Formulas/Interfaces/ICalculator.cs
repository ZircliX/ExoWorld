
namespace LTX.ChanneledProperties.Formulas.Interfaces
{
    public interface ICalculator<T>
    {
        public T Add(T first, T second);
        public T Subtract(T first, T second);
        public T Divide(T first, T second);
        public T Multiply(T first, T second);
    }
}