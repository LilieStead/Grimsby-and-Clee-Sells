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
            //gets all admins
            return _context.Tbl_Admin.ToList();
        }


        public Admin GetAdminByID(int id)
        {
            //gets the first admin with that id
            return _context.Tbl_Admin.FirstOrDefault(u => u.admin_id == id);
        }

        public Admin GetAdminByUsername(string username) 
        {
            //gets the first admin with a username
            return _context.Tbl_Admin.FirstOrDefault(u => u.admin_username == username);
        }

        public async Task<List<User>> GetUsersBySearch(string user)
        {//get the admin that has a user name that contains the user namename, firstname, lastname, email
            return await _context.Tbl_Users.Where(u => (u.users_username.Contains(user) || u.users_firstname.Contains(user) || u.users_lastname.Contains(user) || u.users_email == user)).ToListAsync();
        }
        public Admin GetAdminByPhone(string phone)
            //get admin based on thier phone number 
        {
            return _context.Tbl_Admin.FirstOrDefault(p => p.admin_phone == phone);
        }
        public Admin GetAdminByEmail(string email)
        //get admin based on thier email
        {
            return _context.Tbl_Admin.FirstOrDefault(p => p.admin_email == email);
        }

        public Admin CreateAdmin(Admin admin)
        {
            //add detials to admin table
            _context.Tbl_Admin.Add(admin);
            //save changes to table 
            _context.SaveChanges();
            return admin;
        }
    }
}
