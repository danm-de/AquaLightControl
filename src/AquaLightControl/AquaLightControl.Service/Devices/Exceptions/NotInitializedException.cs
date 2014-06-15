using System;
using System.Runtime.Serialization;

namespace AquaLightControl.Service.Devices
{
    public class NotInitializedException : Exception {
        public NotInitializedException() {}
        public NotInitializedException(string message) : base(message) {}
        public NotInitializedException(string message, Exception inner_exception) : base(message, inner_exception) {}
        protected NotInitializedException(SerializationInfo info, StreamingContext context) : base(info, context) {}
    }
}