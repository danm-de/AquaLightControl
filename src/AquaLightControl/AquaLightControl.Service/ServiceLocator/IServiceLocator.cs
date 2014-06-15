namespace AquaLightControl.Service.ServiceLocator
{
    public interface IServiceLocator
    {
        IDisposableInstance<T> Resolve<T>();
    }
}