namespace AquaLightControl.ServiceLocator
{
    public interface IServiceLocator
    {
        IDisposableInstance<T> Resolve<T>();
    }
}