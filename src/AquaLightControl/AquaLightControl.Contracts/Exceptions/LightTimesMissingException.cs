using System;
using System.Runtime.Serialization;

namespace AquaLightControl
{
    [Serializable]
    public class LightTimesMissingException : Exception {
        public LightTimesMissingException() {}
        public LightTimesMissingException(string message) : base(message) {}
        public LightTimesMissingException(string message, Exception inner_exception) : base(message, inner_exception) {}
        protected LightTimesMissingException(SerializationInfo info, StreamingContext context) : base(info, context) {}
    }
}