using Grimsby_and_Clee_Sells.Models.Domain;

namespace Grimsby_and_Clee_Sells.Repositories
{
    public interface IUserRepository
    {

        List <User> GetAllUsers();

        User UserSignUp(User user);

        User GetUserByUsername (string username);

        User GetUserByID (int id);
    }
}