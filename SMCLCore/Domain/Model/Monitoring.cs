using System;
using System.Collections.Generic;

namespace SMCLCore.Domain.Model
{
    [Serializable]
    public class Monitoring
    {
        public Monitoring()
        {
        }

        public virtual int Id { get; set; }

        public virtual float Value { get; set; }

        public virtual DateTime DateTime { get; set; }

        public virtual string CommentsOnAlarm { get; set; }

        public virtual MappingTag MappingTag { get; set; }

        public virtual User User { get; set; }

        public virtual DateTime CommentsOnAlarmDate { get; set; }

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            Monitoring monitoring = obj as Monitoring;
            if (monitoring == null) return false;
            return Id == monitoring.Id && Equals(MappingTag, monitoring.MappingTag);
        }

        public override int GetHashCode()
        {
            return Id + 29 * MappingTag.GetHashCode();
        }
    }
}