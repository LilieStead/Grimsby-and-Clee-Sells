using Grimsby_and_Clee_Sells.Data;
using Grimsby_and_Clee_Sells.Models.Domain;
using Grimsby_and_Clee_Sells.Models.DTOs;
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
        {//get all cart items
            return _context.Tbl_Cart.Include(p => p.User).Include(p => p.product).ToList();
        }

        public Cartitem AddToCart(Cartitem cartitem)
        {//add cart item
            _context.Tbl_Cart.Add(cartitem);
            _context.SaveChanges();
            return cartitem;
        }

        public Cartitem SearchUserAndProduct(int userid, int  productId) {
            //get item based on product and user 
            return _context.Tbl_Cart.Where(p => p.cart_userid == userid && p.cart_productid == productId).FirstOrDefault();
        }

        public List<Cartitem> GetCartitemByUserId(int userid)
        {//get the cart item based on user id 
            return _context.Tbl_Cart.Where(p => p.cart_userid == userid).Include(p => p.product).ToList();
        }
        public Cartitem DeleteCartitem(int userid, int productid)
        {//delete the first cart item based on the user and product 
            var deleteproduct = _context.Tbl_Cart.Where(p => p.cart_userid == userid).FirstOrDefault(p => p.cart_productid == productid);
            _context.Tbl_Cart.Remove(deleteproduct);
            _context.SaveChanges();

            return deleteproduct;
        }

        public Cartitem UpdateCartItem(int quantity, int userid, int productid)
        {   //update the first cart item that based of the productid and user id
            var item = _context.Tbl_Cart.SingleOrDefault(p => p.cart_userid == userid && p.cart_productid == productid);
            item.cart_quantity = quantity;
            _context.SaveChanges();
            return item;
        }

        public Cartitem GetCartItemByID(int id, int userID)
        {   // return the cart item based on the user and product
            return _context.Tbl_Cart.FirstOrDefault(p => p.cart_productid == id && p.cart_userid == userID);
        }
    }

}
