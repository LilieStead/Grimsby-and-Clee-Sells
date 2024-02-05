using Grimsby_and_Clee_Sells.Models.Domain;

namespace Grimsby_and_Clee_Sells.Repositories
{
    public interface IUserRepository
    {

        List <User> GetAllUsers();

        User UserSignUp(User user);

        User GetUserByUsername (string username);
        User UpdateUserDetails(int userID, User user);
        User RemoveAmount(double amount, int userID);
        User GetUserByID (int id);
    }
}