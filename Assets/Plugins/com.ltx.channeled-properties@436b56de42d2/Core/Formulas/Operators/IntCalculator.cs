using LTX.ChanneledProperties.Formulas.Interfaces;

namespace LTX.ChanneledProperties.Formulas.Operators
{
    internal struct IntCalculator : ICalculator<int>
    {
        public int Add(int first, int second)
        {
            return first + second;
        }

        public int Divide(int first, int second)
        {
            return first / second;
        }

        public int Multiply(int first, int second)
        {
            return first * second;
        }

        public int Subtract(int first, int second)
        {
            return first - second;
        }
    }
}