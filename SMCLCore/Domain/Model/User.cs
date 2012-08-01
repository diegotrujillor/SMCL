using System;
using System.Collections.Generic;

namespace SMCLCore.Domain.Model
{
    [Serializable]
    public class User
    {
        public User()
        {
            Logs = new List<Log>();
            Roles = new List<UserRole>();
            Monitorings = new List<Monitoring>();
        }

        public virtual int Id { get; set; }

        public virtual double DocumentId { get; set; }

        public virtual string LoginEmail { get; set; }

        public virtual string FirstName { get; set; }

        public virtual string MiddleName { get; set; }

        public virtual string LastName1 { get; set; }

        public virtual string LastName2 { get; set; }

        public virtual string PhoneNumber { get; set; }

        public virtual bool IsActive { get; set; }

        public virtual string Password { get; set; }

        public virtual int PassphraseId { get; set; }

        public virtual string PassphraseValue { get; set; }

        public virtual bool IsLoggedFirstTime { get; set; }

        public virtual IList<Log> Logs { get; set; }

        public virtual IList<UserRole> Roles { get; set; }

        public virtual IList<Monitoring> Monitorings { get; set; }

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            User user = obj as User;
            if (user == null) return false;
            return Id == user.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
