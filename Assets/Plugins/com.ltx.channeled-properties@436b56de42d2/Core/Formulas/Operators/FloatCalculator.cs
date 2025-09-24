using LTX.ChanneledProperties.Formulas.Interfaces;

namespace LTX.ChanneledProperties.Formulas.Operators
{
    internal struct FloatCalculator : ICalculator<float>
    {
        public float Add(float first, float second)
        {
            return first + second;
        }

        public float Subtract(float first, float second)
        {
            return first - second;
        }

        public float Divide(float first, float second)
        {
            return first / second;
        }

        public float Multiply(float first, float second)
        {
            return first * second;
        }
    }
}