using Minimal_JWT.Models;

namespace Minimal_JWT.Repositories
{
    public class UserRepository
    {
        public static List<User> Users = new()
        {
            new() {Username = "luke_admin", EmailAddress = "luke.admin@gmail.com", Password = "MyPassword",
                GivenName = "Luke", Surname = "Rogers", Role = "Administrator"},

            new() {Username = "lydia_standard", EmailAddress = "lydia.standard@gmail.com", Password = "MyPassword",
                GivenName = "Elyse", Surname = "Burton", Role = "Standard"},
        };
    }
}
