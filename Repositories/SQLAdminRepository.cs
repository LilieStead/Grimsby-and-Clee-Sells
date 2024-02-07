using Grimsby_and_Clee_Sells.Data;
using Grimsby_and_Clee_Sells.Models.Domain;
using Grimsby_and_Clee_Sells.Models.DTOs;
using Microsoft.EntityFrameworkCore;
namespace Grimsby_and_Clee_Sells.Repositories
{
    public class SQLAdminRepository: IAdminRepository
    {
        private readonly GacsDbContext _context;

        public SQLAdminRepository(GacsDbContext context)
        {
            this._context = context;
        }

        public List<Admin> GetAllAdmins()
        {
            return _context.Tbl_Admin.ToList();
        }


        public Admin GetAdminByID(int id)
        {
            return _context.Tbl_Admin.FirstOrDefault(u => u.admin_id == id);
        }

        public Admin GetAdminByUsername(string username) 
        {
            return _context.Tbl_Admin.FirstOrDefault(u => u.admin_username == username);
        }

        public async Task<List<User>> GetUsersBySearch(string user)
        {
            return await _context.Tbl_Users.Where(u => (u.users_username.Contains(user) || u.users_firstname.Contains(user) || u.users_lastname.Contains(user) || u.users_email == user)).ToListAsync();
        }
        public Admin GetAdminByPhone(string phone)
        {
            return _context.Tbl_Admin.FirstOrDefault(p => p.admin_phone == phone);
        }
        public Admin GetAdminByEmail(string email)
        {
            return _context.Tbl_Admin.FirstOrDefault(p => p.admin_email == email);
        }

        public Admin CreateAdmin(Admin admin)
        {
            _context.Tbl_Admin.Add(admin);
            _context.SaveChanges();
            return admin;
        }
    }
}
