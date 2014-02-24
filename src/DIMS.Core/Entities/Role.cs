using System;
using System.Collections.Generic;

namespace DIMS.Core.Entities
{
    public class Role
    {
        private readonly IList<User> _usersInRole = new List<User>();
        private int _id;

        public virtual string Name { get; set; }

        public virtual int Id
        {
            get { return _id; }
        }

        public virtual string ApplicationName { get; set; }

        public virtual IList<User> UsersInRole
        {
            get { return _usersInRole; }
        }

        public virtual void AddUserToRole(User user)
        {
            if (UsersInRole.Contains(user))
                throw new InvalidOperationException("This user is already assigned the role.");

            _usersInRole.Add(user);
        }

        public virtual void RemoveUserFromRole(User user)
        {
            if (!UsersInRole.Contains(user))
                throw new InvalidOperationException("This user is not in the role.");

            _usersInRole.Remove(user);
        }

        public Role(string name)
        {
            Name = name;
        }

        protected Role() { }
    }
}