using System;
using AquaLightControl.ServiceLocator;

namespace AquaLightControl.Service.ServiceLocator
{
    internal sealed class ResolvedInstance<T> : IDisposableInstance<T>
    {
        private readonly T _instance;
        private readonly Action<T> _dispose_action;
        private bool _disposed;
        
        public T Value {
            get { return _instance; }
        }

        public ResolvedInstance(T instance, Action<T> dispose_action) {
            if (ReferenceEquals(dispose_action, null)) {
                throw new ArgumentNullException("dispose_action");
            }

            _instance = instance;
            _dispose_action = dispose_action;
        }

        public void Dispose() {
            if (_disposed) {
                return;
            }

            _dispose_action(_instance);

            _disposed = true;
        }
    }
}