using System;
using System.Collections.Generic;

namespace SMCLCore.Domain.Model
{
    [Serializable]
    public class AlarmType
    {
        public AlarmType()
        {
            SignalApplianceValues = new List<SignalApplianceValue>();
            MappingTags = new List<MappingTag>();
        }

        public virtual int Id { get; set; }

        public virtual string NameAlarmType { get; set; }

        public virtual string Description { get; set; }

        public virtual IList<SignalApplianceValue> SignalApplianceValues { get; set; }

        public virtual IList<MappingTag> MappingTags { get; set; }

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            AlarmType alarmType = obj as AlarmType;
            if (alarmType == null) return false;
            return Id == alarmType.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
