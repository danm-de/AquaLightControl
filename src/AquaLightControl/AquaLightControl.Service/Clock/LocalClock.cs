using System;

namespace AquaLightControl.Service.Clock
{
    public sealed class LocalClock : IReferenceClock
    {
        public DateTimeOffset GetLocalTime() {
            return DateTimeOffset.Now;
        }
    }
}