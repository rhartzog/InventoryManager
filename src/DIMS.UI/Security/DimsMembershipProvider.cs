using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Configuration.Provider;
using System.Data.Odbc;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Configuration;
using System.Web.Security;
using DIMS.Core.Entities;
using DIMS.Data.DataAccess;
using Ninject;

namespace DIMS.UI.Security
{
    public sealed class DimsMembershipProvider : MembershipProvider
    {
        [Inject]
        public IRepository<User> UserRepository { get; set; }

        #region Private
        
        private const int NewPasswordLength = 8;
        private const string EventSource = "DimsMembershipProvider";
        private const string EventLog = "Application";
        private const string ExceptionMessage = "An exception occurred. Please check the Event Log.";

        private string _applicationName;
        private bool _enablePasswordReset;
        private bool _enablePasswordRetrieval;
        private bool _requiresQuestionAndAnswer;
        private bool _requiresUniqueEmail;
        private int _maxInvalidPasswordAttempts;
        private int _passwordAttemptWindow;
        private MembershipPasswordFormat _passwordFormat;
        // Used when determining encryption key values.
        private MachineKeySection _machineKey;
        private int _minRequiredNonAlphanumericCharacters;
        private int _minRequiredPasswordLength;
        private string _passwordStrengthRegularExpression;

        #endregion

        #region Public Propeties
        public override string ApplicationName
        {
            get { return _applicationName; }
            set { _applicationName = value; }
        }

        public override bool EnablePasswordReset
        {
            get { return _enablePasswordReset; }
        }


        public override bool EnablePasswordRetrieval
        {
            get { return _enablePasswordRetrieval; }
        }


        public override bool RequiresQuestionAndAnswer
        {
            get { return _requiresQuestionAndAnswer; }
        }


        public override bool RequiresUniqueEmail
        {
            get { return _requiresUniqueEmail; }
        }


        public override int MaxInvalidPasswordAttempts
        {
            get { return _maxInvalidPasswordAttempts; }
        }


        public override int PasswordAttemptWindow
        {
            get { return _passwordAttemptWindow; }
        }


        public override MembershipPasswordFormat PasswordFormat
        {
            get { return _passwordFormat; }
        }


        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return _minRequiredNonAlphanumericCharacters; }
        }

        public override int MinRequiredPasswordLength
        {
            get { return _minRequiredPasswordLength; }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { return _passwordStrengthRegularExpression; }
        }

        // If false, exceptions are thrown to the caller. If true,
        // exceptions are written to the event log.
        public bool WriteExceptionsToEventLog { get; set; }

        #endregion

        #region Helper functions
        // A Function to retrieve config values from the configuration file
        private string GetConfigValue(string configValue, string defaultValue)
        {
            return String.IsNullOrWhiteSpace(configValue) ? defaultValue : configValue;
        }

        //Fn to create a Membership user from a Entities.Users class
        private MembershipUser GetMembershipUserFromUser(User user)
        {
            var membershipUser = new MembershipUser(Name,
                user.Username,
                user.Id,
                user.Email,
                user.PasswordQuestion,
                user.Comment,
                user.IsApproved,
                user.IsLockedOut,
                user.CreationDate,
                user.LastLoginDate,
                user.LastActivityDate,
                user.LastPasswordChangedDate,
                user.LastLockedOutDate);

            return membershipUser;
        }

        //Fn that performs the checks and updates associated with password failure tracking
        private void UpdateFailureCount(string username, string failureType)
        {
            var windowStart = new DateTime();
            var failureCount = 0;

            try
            {
                var user = GetUserByUsername(username);

                if (user != null)
                {
                    if (failureType == "password")
                    {
                        failureCount = user.FailedPasswordAttemptCount;
                        windowStart = user.FailedPasswordAttemptWindowStart;
                    }

                    if (failureType == "passwordAnswer")
                    {
                        failureCount = user.FailedPasswordAnswerAttemptCount;
                        windowStart = user.FailedPasswordAnswerAttemptWindowStart;
                    }
                }

                var windowEnd = windowStart.AddMinutes(PasswordAttemptWindow);

                if (failureCount == 0 || DateTime.UtcNow > windowEnd)
                {
                    // First password failure or outside of PasswordAttemptWindow. 
                    // Start a new password failure count from 1 and a new window starting now.

                    if (failureType == "password")
                    {
                        if (user != null)
                        {
                            user.FailedPasswordAttemptCount = 1;
                            user.FailedPasswordAttemptWindowStart = DateTime.UtcNow;
                        }
                    }

                    if (failureType == "passwordAnswer")
                    {
                        if (user != null)
                        {
                            user.FailedPasswordAnswerAttemptCount = 1;
                            user.FailedPasswordAnswerAttemptWindowStart = DateTime.UtcNow;
                        }
                    }
                    UserRepository.Update(user);
                    UserRepository.SaveChanges();
                }
                else
                {
                    if (failureCount++ >= MaxInvalidPasswordAttempts)
                    {
                        // Password attempts have exceeded the failure threshold. Lock out
                        // the user.
                        if (user != null)
                        {
                            user.IsLockedOut = true;
                            user.LastLockedOutDate = DateTime.UtcNow;
                            UserRepository.Update(user);
                        }
                        UserRepository.SaveChanges();
                    }
                    else
                    {
                        // Password attempts have not exceeded the failure threshold. Update
                        // the failure counts. Leave the window the same.

                        if (failureType == "password")
                            if (user != null)
                            {
                                user.FailedPasswordAttemptCount = failureCount;
                            }

                        if (failureType == "passwordAnswer")
                            if (user != null)
                            {
                                user.FailedPasswordAnswerAttemptCount = failureCount;
                            }

                        UserRepository.Update(user);
                        UserRepository.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "UpdateFailureCount");
                    throw new ProviderException("Unable to update failure count and window start." + ExceptionMessage);
                }
                
                throw;
            }
        }

        //CheckPassword: Compares password values based on the MembershipPasswordFormat.
        private bool CheckPassword(string password, string dbpassword)
        {
            var pass1 = password;
            var pass2 = dbpassword;

            switch (PasswordFormat)
            {
                case MembershipPasswordFormat.Encrypted:
                    pass2 = UnEncodePassword(dbpassword);
                    break;
                case MembershipPasswordFormat.Hashed:
                    pass1 = EncodePassword(password);
                    break;
            }

            return pass1 == pass2;
        }

        //EncodePassword:Encrypts, Hashes, or leaves the password clear based on the PasswordFormat.
        private string EncodePassword(string password)
        {
            var encodedPassword = password;

            switch (PasswordFormat)
            {
                case MembershipPasswordFormat.Clear:
                    break;
                case MembershipPasswordFormat.Encrypted:
                    encodedPassword =
                      Convert.ToBase64String(EncryptPassword(Encoding.Unicode.GetBytes(password)));
                    break;
                case MembershipPasswordFormat.Hashed:
                    var hash = new HMACSHA1 {Key = HexToByte(_machineKey.ValidationKey)};
                    encodedPassword =
                      Convert.ToBase64String(hash.ComputeHash(Encoding.Unicode.GetBytes(password)));
                    break;
                default:
                    throw new ProviderException("Unsupported password format.");
            }
            return encodedPassword;
        }

        // UnEncodePassword :Decrypts or leaves the password clear based on the PasswordFormat.
        private string UnEncodePassword(string encodedPassword)
        {
            var password = encodedPassword;

            switch (PasswordFormat)
            {
                case MembershipPasswordFormat.Clear:
                    break;
                case MembershipPasswordFormat.Encrypted:
                    password =
                      Encoding.Unicode.GetString(DecryptPassword(Convert.FromBase64String(password)));
                    break;
                case MembershipPasswordFormat.Hashed:
                    throw new ProviderException("Cannot unencode a hashed password.");
                default:
                    throw new ProviderException("Unsupported password format.");
            }

            return password;
        }

        //   Converts a hexadecimal string to a byte array. Used to convert encryption key values from the configuration.    
        private static byte[] HexToByte(string hexString)
        {
            var returnBytes = new byte[hexString.Length / 2];
            for (var i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }

        // WriteToEventLog
        // A helper function that writes exception detail to the event log. Exceptions
        // are written to the event log as a security measure to avoid private database
        // details from being returned to the browser. If a method does not return a status
        // or boolean indicating the action succeeded or failed, a generic exception is also 
        // thrown by the caller.

        private static void WriteToEventLog(Exception e, string action)
        {
            var log = new EventLog {Source = EventSource, Log = EventLog};

            var message = "An exception occurred communicating with the data source.\n\n";
            message += "Action: " + action + "\n\n";
            message += "Exception: " + e;

            log.WriteEntry(message);
        }

        #endregion

        #region Private Methods

        //single fn to get a membership user by key or username
        private MembershipUser GetMembershipUserByKeyOrUser(bool isKeySupplied, string username, object providerUserKey, bool userIsOnline)
        {
            MembershipUser membershipUser = null;

            try
            {
                User user;
                if (isKeySupplied)
                {
                    user = UserRepository.AsQueryable().Single(u => u.Id == (Guid) providerUserKey);
                }
                else
                {
                    user =
                        UserRepository.AsQueryable()
                            .SingleOrDefault(
                                u =>
                                    u.Username.ToLower() == username.ToLower() &&
                                    u.ApplicationName == ApplicationName);
                }

                if (user != null)
                {
                    membershipUser = GetMembershipUserFromUser(user);

                    if (userIsOnline)
                    {
                        user.LastActivityDate = DateTime.UtcNow;
                        UserRepository.Update(user);
                        UserRepository.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                    WriteToEventLog(e, "GetUser(Object, Boolean)");
                throw new ProviderException(ExceptionMessage);
            }
            return membershipUser;
        }

        private User GetUserByUsername(string username)
        {
            User user;
            try
            {
                user =
                    UserRepository.AsQueryable()
                        .Single(u => u.Username == username && u.ApplicationName == ApplicationName);
            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                    WriteToEventLog(e, "UnlockUser");
                throw new ProviderException(ExceptionMessage);
            }

            return user;
        }

        private IEnumerable<User> GetUsers()
        {
            IList<User> users;

            try
            {
                users = UserRepository.AsQueryable().Where(u => u.ApplicationName == ApplicationName).ToList();
            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                    WriteToEventLog(e, "GetUsers");
                throw new ProviderException(ExceptionMessage);
            }

            return users;
        }

        private IList<User> GetUsersLikeUsername(string usernameToMatch)
        {
            IList<User> users;

            try
            {
                users =
                    UserRepository.AsQueryable()
                        .Where(u => u.Username.Contains(usernameToMatch) && u.ApplicationName == ApplicationName)
                        .ToList();
            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                    WriteToEventLog(e, "GetUsersMatchByUsername");
                throw new ProviderException(ExceptionMessage);
            }

            return users;
        }

        private IList<User> GetUsersLikeEmail(string emailToMatch)
        {
            IList<User> users;

            try
            {
                users =
                    UserRepository.AsQueryable()
                        .Where(u => u.Email.Contains(emailToMatch) && u.ApplicationName == ApplicationName)
                        .ToList();
            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                    WriteToEventLog(e, "GetUsersMatchByEmail");
                throw new ProviderException(ExceptionMessage);
            }
            
            return users;
        }
        #endregion

        #region Public methods

        // Initilaize the provider 
        public override void Initialize(string name, NameValueCollection config)
        {
            // Initialize values from web.config.
            if (config == null)
                throw new ArgumentNullException("config");

            if (string.IsNullOrWhiteSpace(name))
                name = "DimsMembershipProvider";

            if (string.IsNullOrWhiteSpace(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Sample Fluent Nhibernate Membership provider");
            }
            // Initialize the abstract base class.
            base.Initialize(name, config);

            _applicationName = GetConfigValue(config["applicationName"], System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
            _maxInvalidPasswordAttempts = Convert.ToInt32(GetConfigValue(config["maxInvalidPasswordAttempts"], "5"));
            _passwordAttemptWindow = Convert.ToInt32(GetConfigValue(config["passwordAttemptWindow"], "10"));
            _minRequiredNonAlphanumericCharacters = Convert.ToInt32(GetConfigValue(config["minRequiredNonAlphanumericCharacters"], "1"));
            _minRequiredPasswordLength = Convert.ToInt32(GetConfigValue(config["minRequiredPasswordLength"], "7"));
            _passwordStrengthRegularExpression = Convert.ToString(GetConfigValue(config["passwordStrengthRegularExpression"], ""));
            _enablePasswordReset = Convert.ToBoolean(GetConfigValue(config["enablePasswordReset"], "true"));
            _enablePasswordRetrieval = Convert.ToBoolean(GetConfigValue(config["enablePasswordRetrieval"], "true"));
            _requiresQuestionAndAnswer = Convert.ToBoolean(GetConfigValue(config["requiresQuestionAndAnswer"], "false"));
            _requiresUniqueEmail = Convert.ToBoolean(GetConfigValue(config["requiresUniqueEmail"], "true"));
            WriteExceptionsToEventLog = Convert.ToBoolean(GetConfigValue(config["writeExceptionsToEventLog"], "true"));


            var tempFormat = config["passwordFormat"] ?? "Hashed";

            switch (tempFormat)
            {
                case "Hashed":
                    _passwordFormat = MembershipPasswordFormat.Hashed;
                    break;
                case "Encrypted":
                    _passwordFormat = MembershipPasswordFormat.Encrypted;
                    break;
                case "Clear":
                    _passwordFormat = MembershipPasswordFormat.Clear;
                    break;
                default:
                    throw new ProviderException("Password format not supported.");
            }


            //
            // Initialize Connection.
            var connectionStringSettings = ConfigurationManager.ConnectionStrings[config["connectionStringName"]];
            if (connectionStringSettings == null || connectionStringSettings.ConnectionString.Trim() == "")
                throw new ProviderException("Connection string cannot be blank.");

            // Get encryption and decryption key information from the configuration.

            //Encryption skipped
            var cfg =
                WebConfigurationManager.OpenWebConfiguration(
                    System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
            _machineKey = (MachineKeySection)cfg.GetSection("system.web/machineKey");

            if (_machineKey.ValidationKey.Contains("AutoGenerate"))
                if (PasswordFormat != MembershipPasswordFormat.Clear)
                    throw new ProviderException("Hashed or Encrypted passwords are not supported with auto-generated keys.");

        }

        // Change password for a user
        public override bool ChangePassword(string username, string oldPwd, string newPwd)
        {
            var rowsAffected = 0;
            if (!ValidateUser(username, oldPwd))
                return false;

            var args = new ValidatePasswordEventArgs(username, newPwd, true);

            OnValidatingPassword(args);

            if (args.Cancel)
                if (args.FailureInformation != null)
                    throw args.FailureInformation;
                else
                    throw new MembershipPasswordException("Change password canceled due to new password validation failure.");

                    try
                    {
                        var user = GetUserByUsername(username);

                        if (user != null)
                        {
                            user.Password = EncodePassword(newPwd);
                            user.LastPasswordChangedDate = DateTime.UtcNow;
                            UserRepository.Update(user);
                            UserRepository.SaveChanges();
                            rowsAffected = 1;
                        }
                    }
                    catch (Exception e)
                    {
                        if (WriteExceptionsToEventLog)
                            WriteToEventLog(e, "ChangePassword");
                        throw new ProviderException(ExceptionMessage);
                    }


            return rowsAffected > 0;
        }

        // Change Password Question And Answer for a user
        public override bool ChangePasswordQuestionAndAnswer(string username,
            string password,
            string newPwdQuestion,
            string newPwdAnswer)
        {
            var rowsAffected = 0;
            if (!ValidateUser(username, password))
                return false;

            try
            {
                var user = GetUserByUsername(username);
                if (user != null)
                {
                    user.PasswordQuestion = newPwdQuestion;
                    user.PasswordAnswer = newPwdAnswer;
                    UserRepository.Update(user);
                    UserRepository.SaveChanges();
                    rowsAffected = 1;
                }
            }
            catch (OdbcException e)
            {
                if (WriteExceptionsToEventLog)
                    WriteToEventLog(e, "ChangePasswordQuestionAndAnswer");
                throw new ProviderException(ExceptionMessage);
            }

            return rowsAffected > 0;
        }

        // Create a new Membership user 
        public override MembershipUser CreateUser(string username,
                 string password,
                 string email,
                 string passwordQuestion,
                 string passwordAnswer,
                 bool isApproved,
                 object providerUserKey,
                 out MembershipCreateStatus status)
        {
            var args = new ValidatePasswordEventArgs(username, password, true);

            OnValidatingPassword(args);
            if (args.Cancel)
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }

            if (RequiresUniqueEmail && GetUserNameByEmail(email) != "")
            {
                status = MembershipCreateStatus.DuplicateEmail;
                return null;
            }

            var user = GetUser(username, false);

            if (user == null)
            {
                var createDate = DateTime.UtcNow;

                //provider user key in our case is auto int
                var newUser = new User(username, EncodePassword(password), email, passwordQuestion, passwordAnswer)
                {
                    IsApproved = isApproved,
                    Comment = "",
                    CreationDate = createDate,
                    LastPasswordChangedDate = createDate,
                    LastActivityDate = createDate,
                    ApplicationName = _applicationName,
                    IsLockedOut = false,
                    LastLockedOutDate = createDate,
                    FailedPasswordAttemptCount = 0,
                    FailedPasswordAttemptWindowStart = createDate,
                    FailedPasswordAnswerAttemptCount = 0,
                    FailedPasswordAnswerAttemptWindowStart = createDate
                };

                try
                {
                    UserRepository.Create(newUser);
                    UserRepository.SaveChanges();
                    status = MembershipCreateStatus.Success;
                }
                catch (Exception e)
                {
                    status = MembershipCreateStatus.ProviderError;
                    if (WriteExceptionsToEventLog)
                        WriteToEventLog(e, "CreateUser");
                }

                //retrive and return user by user name
                return GetUser(username, false);
            }
            
            status = MembershipCreateStatus.DuplicateUserName;
            return null;
        }

        // Delete a user 
        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            var rowsAffected = 0;

            try
            {
                var user = GetUserByUsername(username);
                if (user != null)
                {
                    UserRepository.Delete(user);
                    UserRepository.SaveChanges();
                    rowsAffected = 1;
                }
            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                    WriteToEventLog(e, "DeleteUser");
                throw new ProviderException(ExceptionMessage);
            }

            return rowsAffected > 0;
        }

        // Get all users in db
        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            var users = new MembershipUserCollection();
            totalRecords = 0;
            var counter = 0;
            var startIndex = pageSize*pageIndex;
            var endIndex = startIndex + pageSize - 1;

            try
            {
                totalRecords = UserRepository.AsQueryable().Count(u => u.ApplicationName == ApplicationName);

                if (totalRecords <= 0)
                {
                    return users;
                }

                var allUsers = GetUsers();
                foreach (var user in allUsers)
                {
                    if (counter >= endIndex)
                        break;
                    if (counter >= startIndex)
                    {
                        var membershipUser = GetMembershipUserFromUser(user);
                        users.Add(membershipUser);
                    }
                    counter++;
                }
            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                    WriteToEventLog(e, "GetAllUsers");
                throw new ProviderException(ExceptionMessage);
            }
            return users;
        }

        // Gets a number of online users
        public override int GetNumberOfUsersOnline()
        {
            var onlineSpan = new TimeSpan(0, Membership.UserIsOnlineTimeWindow, 0);
            var compareTime = DateTime.UtcNow.Subtract(onlineSpan);
            int numOnline;

            try
            {
                numOnline =
                    UserRepository.AsQueryable()
                        .Count(u => u.ApplicationName == ApplicationName && u.LastActivityDate > compareTime);
            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                    WriteToEventLog(e, "GetNumberOfUsersOnline");
                throw new ProviderException(ExceptionMessage);
            }
            return numOnline;
        }

        // Get a password for a user
        public override string GetPassword(string username, string answer)
        {
            string password;
            string passwordAnswer;

            if (!EnablePasswordRetrieval)
                throw new ProviderException("Password Retrieval Not Enabled.");

            if (PasswordFormat == MembershipPasswordFormat.Hashed)
                throw new ProviderException("Cannot retrieve Hashed passwords.");

            try
            {
                var user = GetUserByUsername(username);

                if (user == null)
                    throw new MembershipPasswordException("The supplied user name is not found.");
                
                if (user.IsLockedOut)
                    throw new MembershipPasswordException("The supplied user is locked out.");

                password = user.Password;
                passwordAnswer = user.PasswordAnswer;
            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                    WriteToEventLog(e, "GetPassword");
                throw new ProviderException(ExceptionMessage);
            }

            if (RequiresQuestionAndAnswer && !CheckPassword(answer, passwordAnswer))
            {
                UpdateFailureCount(username, "passwordAnswer");
                throw new MembershipPasswordException("Incorrect password answer.");
            }

            if (PasswordFormat == MembershipPasswordFormat.Encrypted)
                password = UnEncodePassword(password);

            return password;
        }

        // Get a membership user by username
        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            return GetMembershipUserByKeyOrUser(false, username, 0, userIsOnline);
        }

        //  // Get a membership user by key ( in our case key is int)
        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            return GetMembershipUserByKeyOrUser(true, string.Empty, providerUserKey, userIsOnline);
        }

        //Unlock a user given a username 
        public override bool UnlockUser(string username)
        {
            var unlocked = false;

            try
            {
                var user = GetUserByUsername(username);

                if (user != null)
                {
                    user.LastLockedOutDate = DateTime.UtcNow;
                    user.IsLockedOut = false;
                    UserRepository.Update(user);
                    UserRepository.SaveChanges();
                    unlocked = true;
                }
            }
            catch (Exception e)
            {
                WriteToEventLog(e, "UnlockUser");
                throw new ProviderException(ExceptionMessage);
            }

            return unlocked;
        }

        //Gets a membehsip user by email
        public override string GetUserNameByEmail(string email)
        {
            User user;

            try
            {
                user = UserRepository.AsQueryable().SingleOrDefault(u => u.Email == email);
            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                    WriteToEventLog(e, "GetUserNameByEmail");
                throw new ProviderException(ExceptionMessage);
            }

            return user == null ? string.Empty : user.Username;
        }

        // Reset password for a user
        public override string ResetPassword(string username, string answer)
        {
            int rowsAffected;

            if (!EnablePasswordReset)
                throw new NotSupportedException("Password reset is not enabled.");

            if (answer == null && RequiresQuestionAndAnswer)
            {
                UpdateFailureCount(username, "passwordAnswer");
                throw new ProviderException("Password answer required for password reset.");
            }

            var newPassword =
                            Membership.GeneratePassword(NewPasswordLength, MinRequiredNonAlphanumericCharacters);


            var args = new ValidatePasswordEventArgs(username, newPassword, true);

            OnValidatingPassword(args);

            if (args.Cancel)
                if (args.FailureInformation != null)
                    throw args.FailureInformation;
                else
                    throw new MembershipPasswordException("Reset password canceled due to password validation failure.");

            const string passwordAnswer = "";

            try
            {
                var user = GetUserByUsername(username);
                if (user == null)
                    throw new MembershipPasswordException("The supplied user name is not found.");

                if (user.IsLockedOut)
                    throw new MembershipPasswordException("The supplied user is locked out.");

                if (RequiresQuestionAndAnswer && !CheckPassword(answer, passwordAnswer))
                {
                    UpdateFailureCount(username, "passwordAnswer");
                    throw new MembershipPasswordException("Incorrect password answer.");
                }

                user.Password = EncodePassword(newPassword);
                user.LastPasswordChangedDate = DateTime.UtcNow;
                user.Username = username;
                user.ApplicationName = ApplicationName;
                UserRepository.Update(user);
                UserRepository.SaveChanges();
                rowsAffected = 1;
            }
            catch (OdbcException e)
            {
                if (WriteExceptionsToEventLog)
                    WriteToEventLog(e, "ResetPassword");
                throw new ProviderException(ExceptionMessage);
            }

            if (rowsAffected > 0)
                return newPassword;
            
            throw new MembershipPasswordException("User not found, or user is locked out. Password not Reset.");
        }

        // Update a user information 
        public override void UpdateUser(MembershipUser membershipUser)
        {
            try
            {
                var user = GetUserByUsername(membershipUser.UserName);
                
                if (user == null) return;
                
                user.Email = membershipUser.Email;
                user.Comment = membershipUser.Comment;
                user.IsApproved = membershipUser.IsApproved;
                UserRepository.Update(user);
                UserRepository.SaveChanges();
            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "UpdateUser");
                    throw new ProviderException(ExceptionMessage);
                }
            }
        }

        // Validates as user
        public override bool ValidateUser(string username, string password)
        {
            var isValid = false;

            try
            {
                var user = GetUserByUsername(username);
                if (user == null)
                    return false;
                if (user.IsLockedOut)
                    return false;

                if (CheckPassword(password, user.Password))
                {
                    if (user.IsApproved)
                    {
                        isValid = true;
                        user.LastLoginDate = DateTime.UtcNow;
                        UserRepository.Update(user);
                        UserRepository.SaveChanges();
                    }
                }
                else
                    UpdateFailureCount(username, "password");
            }
            catch (Exception e)
            {
                if (!WriteExceptionsToEventLog) throw;
                
                WriteToEventLog(e, "ValidateUser");
                throw new ProviderException(ExceptionMessage);
            }
            return isValid;
        }

        // Find users by a name, note : does not do a like search
        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            var users = new MembershipUserCollection();
            var counter = 0;
            var startIndex = pageSize * pageIndex;
            var endIndex = startIndex + pageSize - 1;
            totalRecords = 0;

            try
            {
                var allUsers = GetUsersLikeUsername(usernameToMatch);
                if (allUsers == null)
                    return users;
                if (allUsers.Count > 0)
                    totalRecords = allUsers.Count;
                else
                    return users;

                foreach (var user in allUsers)
                {
                    if (counter >= endIndex)
                        break;
                    if (counter >= startIndex)
                    {
                        var membershipUser = GetMembershipUserFromUser(user);
                        users.Add(membershipUser);
                    }
                    counter++;
                }
            }
            catch (Exception e)
            {
                if (!WriteExceptionsToEventLog) throw;
                
                WriteToEventLog(e, "FindUsersByName");
                throw new ProviderException(ExceptionMessage);
            }

            return users;
        }

        // Search users by email , NOT a Like match
        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            var users = new MembershipUserCollection();
            var counter = 0;
            var startIndex = pageSize * pageIndex;
            var endIndex = startIndex + pageSize - 1;
            totalRecords = 0;

            try
            {
                var allUsers = GetUsersLikeEmail(emailToMatch);
                if (allUsers == null)
                    return users;
                if (allUsers.Count > 0)
                    totalRecords = allUsers.Count;
                else
                    return users;

                foreach (var user in allUsers)
                {
                    if (counter >= endIndex)
                        break;
                    if (counter >= startIndex)
                    {
                        var membershipUser = GetMembershipUserFromUser(user);
                        users.Add(membershipUser);
                    }
                    counter++;
                }
            }
            catch (Exception e)
            {
                if (!WriteExceptionsToEventLog) throw;
                
                WriteToEventLog(e, "FindUsersByEmail");
                throw new ProviderException(ExceptionMessage);
            }
            return users;
        }

        #endregion
    }
}
