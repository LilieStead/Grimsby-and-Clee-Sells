using Grimsby_and_Clee_Sells.Data;
using Grimsby_and_Clee_Sells.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace Grimsby_and_Clee_Sells.Repositories
{
    public class SQLCartitemRepository : ICartitemRepository
    {
        private readonly GacsDbContext _context;

        public SQLCartitemRepository(GacsDbContext context)
        {
            this._context = context;
        }
        public List<Cartitem> GetAllCartitem()
        {
            return _context.Tbl_Cart.Include(p => p.User).Include(p => p.product).ToList();
        }

        public Cartitem AddToCart(Cartitem cartitem)
        {
            _context.Tbl_Cart.Add(cartitem);
            _context.SaveChanges();
            return cartitem;
        }

        public Cartitem SearchUserAndProduct(int userid, int  productId) {
            return _context.Tbl_Cart.Where(p => p.cart_userid == userid && p.cart_productid == productId).FirstOrDefault();
        }

        public List<Cartitem> GetCartitemByUserId(int userid)
        {
            return _context.Tbl_Cart.Where(p => p.cart_userid == userid).Include(p => p.product).ToList();
        }
        public Cartitem DeleteCartitem(int userid, int productid) 
        {
            var deleteproduct = _context.Tbl_Cart.Where(p => p.cart_userid == userid).FirstOrDefault(p => p.cart_productid == productid);
            _context.Tbl_Cart.Remove(deleteproduct);
            _context.SaveChanges();

            return deleteproduct;
        }
    }
}
