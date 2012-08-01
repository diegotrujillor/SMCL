using System;

namespace SMCLCore.Domain.Model
{
    [Serializable]
    public class UserRole
    {
        public virtual Guid Id { get; set; }

        public virtual User User { get; set; }

        public virtual Role Role { get; set; }

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            UserRole userRole = obj as UserRole;
            if (userRole == null) return false;
            return Id == userRole.Id && Equals(User, userRole.User);
        }

        public override int GetHashCode()
        {
            return User.GetHashCode();
        }
    }
}
