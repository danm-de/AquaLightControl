namespace AquaLightControl.Service.Clock
{
    public sealed class Clock : IClock
    {
        private readonly IReferenceClock _reference_clock;

        public Clock(IReferenceClock reference_clock) {
            _reference_clock = reference_clock;
        }

        public long GetTicksForTimeOfDay() {
            var current_time = _reference_clock.GetLocalTime();
            return current_time.TimeOfDay.Ticks;
        }
    }
}