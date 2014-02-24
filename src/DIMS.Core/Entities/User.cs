using System;
using System.Collections.Generic;

namespace DIMS.Core.Entities
{
    public class User
    {
        private readonly IList<Role> _roles = new List<Role>();
        private Guid _id;

        public virtual Guid Id
        {
            get { return _id; }
        }

        public virtual string Username { get; set; }
        public virtual string ApplicationName { get; set; }
        public virtual string Email { get; set; }
        public virtual string Comment { get; set; }
        public virtual string Password { get; set; }
        public virtual string PasswordQuestion { get; set; }
        public virtual string PasswordAnswer { get; set; }
        public virtual bool  IsApproved { get; set; }
        public virtual DateTime LastActivityDate { get; set; }
        public virtual DateTime LastLoginDate { get; set; }
        public virtual DateTime LastPasswordChangedDate { get; set; }
        public virtual DateTime CreationDate { get; set; }
        public virtual bool IsOnLine { get; set; }
        public virtual bool IsLockedOut { get; set; }
        public virtual DateTime LastLockedOutDate { get; set; }
        public virtual int FailedPasswordAttemptCount { get; set; }
        public virtual int FailedPasswordAnswerAttemptCount { get; set; }
        public virtual DateTime FailedPasswordAttemptWindowStart { get; set; }
        public virtual DateTime FailedPasswordAnswerAttemptWindowStart { get; set; }

        public virtual IList<Role> Roles
        {
            get { return _roles; }
        }

        public User(string username, string password, string email, string passwordQuestion, string passwordAnswer)
        {
            Username = username;
            Password = password;
            Email = email;
            PasswordQuestion = passwordQuestion;
            PasswordAnswer = passwordAnswer;
            CreationDate = DateTime.UtcNow;
            LastPasswordChangedDate = DateTime.UtcNow;
            LastActivityDate = DateTime.UtcNow;
            LastLockedOutDate = DateTime.UtcNow;
            FailedPasswordAnswerAttemptWindowStart = DateTime.UtcNow;
            FailedPasswordAttemptWindowStart = DateTime.UtcNow;
            LastLoginDate = DateTime.UtcNow;
        }

        protected User() { }

        public virtual void AddToRole(Role role)
        {
            if (Roles.Contains(role))
                throw new InvalidOperationException("The user already has the specified role.");

            role.UsersInRole.Add(this);
            Roles.Add(role);
        }

        public virtual void RemoveFromRole(Role role)
        {
            if(!Roles.Contains(role))
                throw new InvalidOperationException("The user is not in the specified role.");

            role.UsersInRole.Remove(this);
            Roles.Remove(role);
        }

    }
}