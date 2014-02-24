using DIMS.Core.Entities;
using FluentNHibernate;
using FluentNHibernate.Mapping;

namespace DIMS.Data
{
    public class UserMap : ClassMap<User>
    {
        public UserMap()
        {
            Id(x => x.Id).Access.ReadOnlyPropertyThroughCamelCaseField(Prefix.Underscore);
            Map(x => x.Username);
            Map(x => x.ApplicationName);
            Map(x => x.Email);
            Map(x => x.Comment);
            Map(x => x.Password);
            Map(x => x.PasswordQuestion);
            Map(x => x.PasswordAnswer);
            Map(x => x.IsApproved);
            Map(x => x.LastActivityDate);
            Map(x => x.LastLoginDate);
            Map(x => x.LastPasswordChangedDate);
            Map(x => x.CreationDate);
            Map(x => x.IsOnLine);
            Map(x => x.IsLockedOut);
            Map(x => x.LastLockedOutDate);
            Map(x => x.FailedPasswordAttemptCount);
            Map(x => x.FailedPasswordAnswerAttemptCount);
            Map(x => x.FailedPasswordAttemptWindowStart);
            Map(x => x.FailedPasswordAnswerAttemptWindowStart);

            HasManyToMany(x => x.Roles)
                    .Cascade.All()
                    .Table("UsersInRoles")
                    .Not.LazyLoad();
        }
    }
}