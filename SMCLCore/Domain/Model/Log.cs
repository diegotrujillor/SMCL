using System;
using System.Collections.Generic;

namespace SMCLCore.Domain.Model
{
    [Serializable]
    public class Log
    {
        public Log()
        {
        }

        public virtual int Id { get; set; }

        public virtual DateTime DateTime { get; set; }

        public virtual string Text { get; set; }

        public virtual Event Event { get; set; }

        public virtual User User { get; set; }

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            Log log = obj as Log;
            if (log == null) return false;
            return Id == log.Id && Equals(Event, log.Event);
        }

        public override int GetHashCode()
        {
            return Id + 29 * Event.GetHashCode();
        }
    }
}
