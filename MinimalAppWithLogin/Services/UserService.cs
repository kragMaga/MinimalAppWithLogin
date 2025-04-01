using Minimal_JWT.Models;
using Minimal_JWT.Repositories;

namespace Minimal_JWT.Services
{
    public class UserService : IUserService
    {
        public User Get(UserLogin userLogin)
        {
            User user = UserRepository.Users.FirstOrDefault(e => e.Username.Equals(userLogin.Username,
                StringComparison.OrdinalIgnoreCase) && e.Password.Equals(userLogin.Password));

            return user;
        }
    }
}
