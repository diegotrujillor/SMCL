using System;
using System.Collections.Generic;

namespace SMCLCore.Domain.Model
{
    [Serializable]
    public class SignalAppliance
    {
        public SignalAppliance()
        {
            SignalApplianceValues = new List<SignalApplianceValue>();
        }

        public virtual Guid Id { get; set; }

        public virtual Signal Signal { get; set; }

        public virtual Appliance Appliance { get; set; }

        public virtual  IList<SignalApplianceValue> SignalApplianceValues { get; set; }

        public virtual float Tolerance { get; set; }

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            SignalAppliance signalAppliance = obj as SignalAppliance;
            if (signalAppliance == null) return false;
            return Id == signalAppliance.Id && Equals(Signal, signalAppliance.Signal);
        }

        public override int GetHashCode()
        {
            return Signal.GetHashCode();
        }
    }
}
