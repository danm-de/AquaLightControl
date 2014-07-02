namespace AquaLightControl.Math
{
    public interface ILineFunctionFactory {
        ILineFunction Create(LightLine light_line);
    }
}