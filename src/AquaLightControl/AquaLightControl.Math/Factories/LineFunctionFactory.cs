namespace AquaLightControl.Math.Factories
{
    public sealed class LineFunctionFactory : ILineFunctionFactory
    {
        public ILineFunction Create(Point start, Point end) {
            return new LineFunction(start, end);
        }
    }
}