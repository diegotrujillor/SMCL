using System;
using System.Collections.Generic;

namespace SMCLCore.Domain.Model
{
    [Serializable]
    public class Event
    {
        public Event()
        {
            Logs = new List<Log>();
        }

        public virtual int Id { get; set; }

        public virtual string Name { get; set; }

        public virtual string Description { get; set; }

        public virtual IList<Log> Logs { get; set; }

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            Event eventt = obj as Event;
            if (eventt == null) return false;
            return Id == eventt.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
