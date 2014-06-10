namespace AquaLightControlService.ServiceLocator
{
    public interface IServiceLocator
    {
        IDisposableInstance<T> Resolve<T>();
    }
}