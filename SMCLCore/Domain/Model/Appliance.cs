using System;
using System.Collections.Generic;

namespace SMCLCore.Domain.Model
{
    [Serializable]
    public class Appliance
    {
        public Appliance()
        {
            Signals = new List<SignalAppliance>();
            MappingTags = new List<MappingTag>();
        }

        public virtual int Id { get; set; }

        public virtual string NameAppliance { get; set; }

        public virtual string Description { get; set; }

        public virtual Area Area { get; set; }

        public virtual IList<SignalAppliance> Signals { get; set; }

        public virtual IList<MappingTag> MappingTags { get; set; }

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            Appliance appliance = obj as Appliance;
            if (appliance == null) return false;
            return Id == appliance.Id && Equals(Area, appliance.Area);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
