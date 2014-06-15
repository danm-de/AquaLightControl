using System;

namespace AquaLightControl.ServiceLocator
{
    public interface IDisposableInstance<out T> : IDisposable
    {
        T Value { get; }
    }
}