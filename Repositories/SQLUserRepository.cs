using Grimsby_and_Clee_Sells.Data;
using Grimsby_and_Clee_Sells.Models.Domain;

namespace Grimsby_and_Clee_Sells.Repositories
{
    public class SQLUserRepository: IUserRepository
    {
        private readonly GacsDbContext _context;

        public SQLUserRepository(GacsDbContext context)
        {
            this._context = context;
        }

        public List<User> GetAllUsers()
        {
            return _context.Tbl_Users.ToList();
        }


        public  User UserSignUp(User user)
        {
            _context.Tbl_Users.Add(user);
            _context.SaveChanges();
            return user;
        }

        public User GetUserByUsername(string username)
        {
            return _context.Tbl_Users.FirstOrDefault(u => u.users_username == username);
        }

        public User UpdateUserDetails(int userID, User user)
        {
            var thisUser = _context.Tbl_Users.FirstOrDefault(p => p.users_id == userID);
            if (thisUser == null)
            {
                return null;
            }
            thisUser.users_firstname = user.users_firstname;
            thisUser.users_lastname = user.users_lastname;
            thisUser.users_email = user.users_email;
            thisUser.users_username = user.users_username;
            thisUser.users_phone = user.users_phone;
            thisUser.users_dob = user.users_dob;
            thisUser.users_balance = user.users_balance;
            _context.SaveChanges();
            return thisUser;
        }

        public User RemoveAmount(double amount, int userID)
        {
            var user = _context.Tbl_Users.FirstOrDefault(u => u.users_id == userID);
            if (user == null)
            {
                return null;
            }
            user.users_balance -= amount;
            _context.SaveChanges();
            return user;
        }

        public User GetUserByID(int id)
        {
            return _context.Tbl_Users.FirstOrDefault(u => u.users_id == id);
        }
    }
}
