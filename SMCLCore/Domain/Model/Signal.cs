using System;
using System.Collections.Generic;

namespace SMCLCore.Domain.Model
{
    [Serializable]
    public class Signal
    {
        public Signal()
        {
            Appliances = new List<SignalAppliance>();
            MappingTags = new List<MappingTag>();
        }

        public virtual int Id { get; set; }

        public virtual string Name { get; set; }

        public virtual string Description { get; set; }

        public virtual IList<SignalAppliance> Appliances { get; set; }

        public virtual IList<MappingTag> MappingTags { get; set; }

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            Signal signal = obj as Signal;
            if (signal == null) return false;
            return Id == signal.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
