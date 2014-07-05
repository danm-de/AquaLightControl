using System;

namespace AquaLightControl.Service.Clock
{
    public interface IReferenceClock
    {
        DateTimeOffset GetLocalTime();
    }
}