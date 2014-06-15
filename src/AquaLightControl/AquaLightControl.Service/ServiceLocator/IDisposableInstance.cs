using System;

namespace AquaLightControl.Service.ServiceLocator
{
    public interface IDisposableInstance<out T> : IDisposable
    {
        T Value { get; }
    }
}