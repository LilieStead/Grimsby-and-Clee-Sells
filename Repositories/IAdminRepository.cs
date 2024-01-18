using Grimsby_and_Clee_Sells.Models.Domain;


namespace Grimsby_and_Clee_Sells.Repositories
{
    public interface IAdminRepository
    {
        List <Admin> GetAllAdmins();

        Admin GetAdminByID(int id);

        Admin GetAdminByUsername(string username);

        Task<List<User>> GetUsersBySearch(string user);

    }
}
