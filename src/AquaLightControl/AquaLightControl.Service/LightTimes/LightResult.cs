namespace AquaLightControl.Service.LightTimes
{
    public struct LightResult
    {
        private readonly bool _has_changes;
        private readonly bool _all_off;
        
        public bool HasChanges {
            get { return _has_changes; }
        }
        
        public bool AllOff {
            get { return _all_off; }
        }

        public LightResult(bool has_changes, bool all_off) {
            _has_changes = has_changes;
            _all_off = all_off;
        }
    }
}