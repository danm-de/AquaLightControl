namespace AquaLightControl.Service.LightTimes
{
    public struct LightResult
    {
        private readonly bool _has_changes;
        private readonly bool _power_on;
        
        public bool HasChanges {
            get { return _has_changes; }
        }
        
        public bool PowerOn {
            get { return _power_on; }
        }

        public LightResult(bool has_changes, bool power_on) {
            _has_changes = has_changes;
            _power_on = power_on;
        }
    }
}