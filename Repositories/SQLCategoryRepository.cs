using Grimsby_and_Clee_Sells.Data;
using Grimsby_and_Clee_Sells.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace Grimsby_and_Clee_Sells.Repositories
{
    public class SQLCategoryRepository : ICategoryRepository
    {
        private readonly GacsDbContext _context;

        public SQLCategoryRepository(GacsDbContext context)
        {
            this._context = context;
        }

        public List<Category> GetAllCategory()
        {
            return _context.Tbl_Category.ToList();
        }

    }

}

    