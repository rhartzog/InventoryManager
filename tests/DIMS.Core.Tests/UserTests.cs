using DIMS.Core.Entities;
using Xunit;

namespace DIMS.Core.Tests
{
    public class UserTests
    {
        private const string firstName = "Ryan";
        private const string lastName = "Hartzog";
        private const string userName = "rhartzog";
        private const string password = "password";
        private const string email = "rhartzog@yourtest.com";
        private const string passwordQuestion = "Why me?";
        private const string passwordAnswer = "Because";

        [Fact]
        public void Can_add_a_user_to_a_role()
        {
            var role = new Role("Test Role");
            var user = new User(userName, password, email, passwordQuestion, passwordAnswer);

            user.AddToRole(role);
            
            Assert.True(user.Roles.Contains(role));
        }

        [Fact]
        public void Can_remove_a_user_from_a_role()
        {
            var role = new Role("Test Role");
            var user = new User(userName, password, email, passwordQuestion, passwordAnswer);

            user.AddToRole(role);

            Assert.True(user.Roles.Contains(role));

            user.RemoveFromRole(role);

            Assert.False(user.Roles.Contains(role));
        }
         
    }
}