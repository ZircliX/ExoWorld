namespace LTX.ChanneledProperties.Formulas
{
    public enum Operator
    {
        Add,
        Subtract,
        Multiply,
        Divide,
    }

    public enum MultiplyWith
    {
        Nothing = 0,
        StartValue,
        StartGroupValue,
        CurrentValue,
    }
}