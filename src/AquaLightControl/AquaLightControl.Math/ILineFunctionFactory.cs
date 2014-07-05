namespace AquaLightControl.Math
{
    public interface ILineFunctionFactory {
        ILineFunction Create(Point start, Point end);
    }
}