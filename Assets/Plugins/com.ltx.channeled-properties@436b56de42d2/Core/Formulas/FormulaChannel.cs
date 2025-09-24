namespace LTX.ChanneledProperties.Formulas
{
    public struct FormulaChannel<T> : IChannel<T> where T : unmanaged
    {
        T IChannel<T>.Value
        {
            get => value;
            set => this.value = value;
        }

        public Operator op;
        public MultiplyWith multiplyWith;
        public T value;
        public int group;
        public int orderInGroup;
    }
}