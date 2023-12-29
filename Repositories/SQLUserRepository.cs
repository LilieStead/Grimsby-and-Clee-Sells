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


        public User GetUserByID(int id)
        {
            return _context.Tbl_Users.FirstOrDefault(u => u.users_id == id);
        }
    }
}
