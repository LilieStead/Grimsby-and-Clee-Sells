using Grimsby_and_Clee_Sells.Data;
using Grimsby_and_Clee_Sells.Models.Domain;
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
    }
}
