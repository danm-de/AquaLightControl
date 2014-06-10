using System;

namespace AquaLightControlService.ServiceLocator
{
    public interface IDisposableInstance<out T> : IDisposable
    {
        T Value { get; }
    }
}