using System;

namespace SMCLCore.Domain.Model
{
    [Serializable]
    public class SignalApplianceValue
    {
        public SignalApplianceValue()
        {
        }

        public virtual Guid Id { get; set; }
        
        public virtual float Value { get; set; }

        public virtual SignalAppliance SignalAppliance { get; set; }

        public virtual AlarmType AlarmType { get; set; }

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            SignalApplianceValue signalApplianceValue = obj as SignalApplianceValue;
            if (signalApplianceValue == null) return false;
            return Id == signalApplianceValue.Id && Equals(SignalAppliance, signalApplianceValue.SignalAppliance);
        }

        public override int GetHashCode()
        {
            return SignalAppliance.GetHashCode();
        }
    }
}
