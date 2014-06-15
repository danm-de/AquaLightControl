namespace AquaLightControl.Math
{
    public interface ILineFunctionFactory {
        ILineFunction Create(LightTime light_time);
    }
}