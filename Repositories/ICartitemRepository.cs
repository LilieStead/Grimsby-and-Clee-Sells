using Grimsby_and_Clee_Sells.Models.Domain;
using Grimsby_and_Clee_Sells.Models.DTOs;

namespace Grimsby_and_Clee_Sells.Repositories
{
    public interface ICartitemRepository
    {
        List<Cartitem> GetAllCartitem();
        Cartitem AddToCart(Cartitem cartitem);
        Cartitem SearchUserAndProduct(int userid, int productId);
        List<Cartitem> GetCartitemByUserId(int userid);

        Cartitem DeleteCartitem(int userid, int productid);

        Cartitem UpdateCartItem( int quantity, int userid , int productid);


    }
}
