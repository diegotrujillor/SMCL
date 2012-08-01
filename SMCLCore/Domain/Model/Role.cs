using System;
using System.Collections.Generic;

namespace SMCLCore.Domain.Model
{
    [Serializable]
    public class Role
    {
        public Role()
        {
            Users = new List<UserRole>();
        }

        public virtual int Id { get; set; }

        public virtual string Name { get; set; }

        public virtual string Description { get; set; }

        public virtual IList<UserRole> Users { get; set; }

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            Role role = obj as Role;
            if (role == null) return false;
            return Id == role.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
