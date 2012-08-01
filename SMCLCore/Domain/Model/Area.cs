using System;
using System.Collections.Generic;

namespace SMCLCore.Domain.Model
{
    [Serializable]
    public class Area
    {
        public Area()
        {
            Appliances = new List<Appliance>();
        }

        public virtual int Id { get; set; }

        public virtual string Name { get; set; }

        public virtual string Description { get; set; }

        public virtual IList<Appliance> Appliances { get; set; }

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            Area area = obj as Area;
            if (area == null) return false;
            return Id == area.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
