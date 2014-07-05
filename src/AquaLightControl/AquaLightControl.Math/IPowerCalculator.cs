namespace AquaLightControl.Math
{
    public interface IPowerCalculator : ILineFunction
    {
        long Start { get; }
        long End { get; }
    }
}