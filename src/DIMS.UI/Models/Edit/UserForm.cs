namespace DIMS.UI.Models.Edit
{
    public class UserForm
    {
        public string Username { get; set; } 
        public string Password { get; set; }
        public string Email { get; set; }
        public string PasswordQuestion { get; set; }
        public string PasswordAnswer { get; set; }
        public bool IsApproved { get; set; }
        public object ProviderUserKey { get; set; }
    }
}