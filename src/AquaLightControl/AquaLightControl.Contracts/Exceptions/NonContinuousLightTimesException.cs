using System;
using System.Runtime.Serialization;

namespace AquaLightControl
{
    [Serializable]
    public class NonContinuousLightTimesException : Exception {
        public NonContinuousLightTimesException() {}
        public NonContinuousLightTimesException(string message) : base(message) {}
        public NonContinuousLightTimesException(string message, Exception inner_exception) : base(message, inner_exception) {}
        protected NonContinuousLightTimesException(SerializationInfo info, StreamingContext context) : base(info, context) {}
    }
}