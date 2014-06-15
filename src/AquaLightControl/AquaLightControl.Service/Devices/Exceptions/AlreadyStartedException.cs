using System;
using System.Runtime.Serialization;

namespace AquaLightControl.Service.Devices
{
    [Serializable]
    public class AlreadyStartedException : Exception {
        public AlreadyStartedException() {}
        public AlreadyStartedException(string message) : base(message) {}
        public AlreadyStartedException(string message, Exception inner_exception) : base(message, inner_exception) {}
        protected AlreadyStartedException(SerializationInfo info, StreamingContext context) : base(info, context) {}
    }
}