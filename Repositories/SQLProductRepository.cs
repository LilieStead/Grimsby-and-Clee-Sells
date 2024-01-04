using Grimsby_and_Clee_Sells.Data;
using Grimsby_and_Clee_Sells.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace Grimsby_and_Clee_Sells.Repositories
{

    public class SQLProductRepository : IProductRepository
    {
        private readonly GacsDbContext _context;

        public SQLProductRepository(GacsDbContext context)
        {
            this._context = context;
        }

        public List<Product> GetAllProduct()
        {
            return _context.Tbl_Product.Include(p => p.Status).Include(p =>p.Category).Include(p => p.User).ToList();
        }

    }
    
}
