using Minimal_JWT.Models;

namespace Minimal_JWT.Services
{
    public interface IUserService
    {
        public User Get(UserLogin userLogin);
    }
}
