using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Data.Odbc;
using System.Diagnostics;
using System.Linq;
using System.Web.Security;
using DIMS.Core.Entities;
using DIMS.Data.DataAccess;
using Ninject;

namespace DIMS.UI.Security
{
    public sealed class DimsRoleProvider : RoleProvider
    {
        #region private

        [Inject]
        public IRepository<Role> RoleRepository { get; set; }

        [Inject]
        public IRepository<User> UserRepository { get; set; }

        private const string EventSource = "DimsRoleProvider";
        private const string EventLog = "Application";
        private const string ExceptionMessage = "An exception occurred. Please check the Event Log.";
        private string _applicationName = "DIMS";

        #endregion

        #region Properties

        public override string ApplicationName
        {
            get { return _applicationName; }
            set { _applicationName = value; }
        }

        public bool WriteExceptionsToEventLog { get; set; }

        #endregion

        #region Helper Functions

        // A helper function to retrieve config values from the configuration file
        private static string GetConfigValue(string configValue, string defaultValue)
        {
            return String.IsNullOrEmpty(configValue) ? defaultValue : configValue;
        }

        private static void WriteToEventLog(Exception e, string action)
        {
            var log = new EventLog {Source = EventSource, Log = EventLog};

            var message = ExceptionMessage + "\n\n";
            message += "Action: " + action + "\n\n";
            message += "Exception: " + e;

            log.WriteEntry(message);
        }

        #endregion

        #region Private Methods

        //get a role by name
        private Role GetRole(string roleName)
        {
            Role role = null;
            try
            {
                role =
                    RoleRepository.AsQueryable()
                        .Single(r => String.Equals(r.Name, roleName, StringComparison.CurrentCultureIgnoreCase));

            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                    WriteToEventLog(e, "GetRole");
                else
                    throw;
            }
            return role;
        }

        #endregion

        #region Public Methods

        //initializes the FNH role provider
        public override void Initialize(string name, NameValueCollection config)
        {
            // Initialize values from web.config.

            if (config == null)
                throw new ArgumentNullException("config");

            if (string.IsNullOrWhiteSpace(name))
                name = "DimsRoleProvider";

            if (string.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Sample Fluent Nhibernate Role provider");
            }

            // Initialize the abstract base class.
            base.Initialize(name, config);

            _applicationName = GetConfigValue(config["applicationName"],
                System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
            
            WriteExceptionsToEventLog = Convert.ToBoolean(GetConfigValue(config["writeExceptionsToEventLog"], "true"));
        }

        //adds a user collection toa roles collection
        public override void AddUsersToRoles(string[] userNames, string[] roleNames)
        {
            User user = null;
            foreach (var roleName in roleNames)
            {
                if (!RoleExists(roleName))
                    throw new ProviderException(String.Format("Role name {0} not found.", roleName));
            }

            foreach (var username in userNames)
            {
                if (username.Contains(","))
                    throw new ArgumentException(String.Format("User names {0} cannot contain commas.", username));
                //is user not exiting //throw exception

                foreach (var rolename in roleNames)
                {
                    if (IsUserInRole(username, rolename))
                        throw new ProviderException(String.Format("User {0} is already in role {1}.", username, rolename));
                }
            }

            try
            {
                foreach (var username in userNames)
                {
                    foreach (var rolename in roleNames)
                    {
                        //get the user
                        user =
                            UserRepository.AsQueryable()
                                .Single(u => u.Username == username && u.ApplicationName == ApplicationName);

                        if (user != null)
                        {
                            //get the role first from db
                            Role role =
                                RoleRepository.AsQueryable()
                                    .Single(r => r.Name == rolename && r.ApplicationName == ApplicationName);


                            //Role role = GetRole(rolename);
                            user.AddToRole(role);
                        }
                    }
                    UserRepository.Update(user);
                }
                UserRepository.SaveChanges();
            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                    WriteToEventLog(e, "AddUsersToRoles");
                else
                    throw;
            }
        }

        //create  a new role with a given name
        public override void CreateRole(string roleName)
        {
            if (roleName.Contains(","))
                throw new ArgumentException("Role names cannot contain commas.");

            if (RoleExists(roleName))
                throw new ProviderException("Role name already exists.");

            try
            {
                var role = new Role(roleName) {ApplicationName = ApplicationName};
                RoleRepository.Create(role);
                RoleRepository.SaveChanges();
            }
            catch (OdbcException e)
            {
                if (WriteExceptionsToEventLog)
                    WriteToEventLog(e, "CreateRole");
                else
                    throw;
            }
        }

        //delete a role with given name
        public override bool DeleteRole(string rolename, bool throwOnPopulatedRole)
        {
            const bool deleted = false;
            if (!RoleExists(rolename))
                throw new ProviderException("Role does not exist.");

            if (throwOnPopulatedRole && GetUsersInRole(rolename).Length > 0)
                throw new ProviderException("Cannot delete a populated role.");

            try
            {
                var role = GetRole(rolename);
                RoleRepository.Delete(role);
                RoleRepository.SaveChanges();

            }
            catch (OdbcException e)
            {
                if (!WriteExceptionsToEventLog) throw;

                WriteToEventLog(e, "DeleteRole");
                return deleted;
            }

            return deleted;
        }

        //get an array of all the roles
        public override string[] GetAllRoles()
        {
            var allroles = new List<Role>();

            try
            {
                allroles = RoleRepository.AsQueryable().Where(r => r.ApplicationName == ApplicationName).ToList();
            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                    WriteToEventLog(e, "GetAllRoles");
                else
                    throw;
            }

            return allroles.Select(r => r.Name).ToArray();
        }

        //Get roles for a user by username
        public override string[] GetRolesForUser(string username)
        {
            IList<Role> rolesUserIsIn = new List<Role>();
            try
            {
                var user =
                    UserRepository.AsQueryable()
                        .Single(u => u.Username == username && u.ApplicationName == ApplicationName);

                if (user != null)
                {
                    rolesUserIsIn = user.Roles;
                }
            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                    WriteToEventLog(e, "GetRolesForUser");
                else
                    throw;
            }

            return rolesUserIsIn.Select(r => r.Name).ToArray();
        }

        //Get users in a givenrolename
        public override string[] GetUsersInRole(string rolename)
        {
            IList<User> usersInRole = new List<User>();
            try
            {
                Role role =
                    RoleRepository.AsQueryable().Single(r => r.ApplicationName == ApplicationName && r.Name == rolename);

                usersInRole = role.UsersInRole;
            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                    WriteToEventLog(e, "GetUsersInRole");
                else
                    throw;
            }

            return usersInRole.Select(u => u.Username).ToArray();
        }

        //determine is a user has a given role
        public override bool IsUserInRole(string username, string rolename)
        {
            var userIsInRole = false;
            try
            {
                var user = UserRepository.AsQueryable().Single(u => u.Username == username);

                if (user != null)
                {
                    userIsInRole = user.Roles.Any(r => r.Name == rolename);
                }
            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                    WriteToEventLog(e, "IsUserInRole");
                else
                    throw;
            }
            return userIsInRole;
        }

        //remeove users from roles
        public override void RemoveUsersFromRoles(string[] usernames, string[] rolenames)
        {
            foreach (var rolename in rolenames.Where(rolename => !RoleExists(rolename)))
            {
                throw new ProviderException(String.Format("Role name {0} not found.", rolename));
            }


            foreach (var username in usernames)
            {
                foreach (var rolename in rolenames.Where(rolename => !IsUserInRole(username, rolename)))
                {
                    throw new ProviderException(String.Format("User {0} is not in role {1}.", username, rolename));
                }
            }

            //get user , get his roles , the remove the role and save   

            try
            {
                foreach (var username in usernames)
                {
                    var user = UserRepository.AsQueryable()
                        .Single(u => u.Username == username && u.ApplicationName == ApplicationName);

                    var rolesToDelete = user.Roles.Where(r => rolenames.Contains(r.Name));

                    foreach (var roleToDelete in rolesToDelete)
                    {
                        user.RemoveFromRole(roleToDelete);
                    }

                    UserRepository.Update(user);
                }
                UserRepository.SaveChanges();
            }
            catch (OdbcException e)
            {
                if (WriteExceptionsToEventLog)
                    WriteToEventLog(e, "RemoveUsersFromRoles");
                else
                    throw;
            }

        }

        //boolen to check if a role exists given a role name
        public override bool RoleExists(string rolename)
        {
            var exists = false;
            try
            {
                exists =
                    RoleRepository.AsQueryable().Any(r => r.Name == rolename && r.ApplicationName == ApplicationName);
            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                    WriteToEventLog(e, "RoleExists");
                else
                    throw;
            }
            return exists;
        }

        //find users that beloeng to a particular role , given a username, Note : does not do a LIke search
        public override string[] FindUsersInRole(string rolename, string usernameToMatch)
        {
            IList<User> users = new List<User>();
            try
            {
                var role =
                    RoleRepository.AsQueryable().Single(r => r.Name == rolename && r.ApplicationName == ApplicationName);

                users = role.UsersInRole.Where(u => usernameToMatch.Contains(u.Username)).ToList();
            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                    WriteToEventLog(e, "FindUsersInRole");
                else
                    throw;
            }

            return users.Select(u => u.Username).ToArray();
        }

        #endregion
    }
}