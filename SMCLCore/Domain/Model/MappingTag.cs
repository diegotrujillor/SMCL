using System;
using System.Collections.Generic;

namespace SMCLCore.Domain.Model
{
    [Serializable]
    public class MappingTag
    {
        public MappingTag()
        {
            Monitorings = new List<Monitoring>();
        }

        public virtual int Id { get; set; }

        public virtual string Tag { get; set; }

        public virtual string Description { get; set; }

        public virtual Signal Signal { get; set; }

        public virtual Appliance Appliance { get; set; }

        public virtual AlarmType AlarmType { get; set; }

        public virtual IList<Monitoring> Monitorings { get; set; }

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            MappingTag mappingTag = obj as MappingTag;
            if (mappingTag == null) return false;
            return Id == mappingTag.Id && Equals(Signal, mappingTag.Signal);
        }

        public override int GetHashCode()
        {
            return Id + 29 * Signal.GetHashCode();
        }
    }
}
