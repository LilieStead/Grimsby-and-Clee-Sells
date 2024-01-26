using Grimsby_and_Clee_Sells.Models.Domain;

namespace Grimsby_and_Clee_Sells.Repositories
{
    public interface ICartitemRepository
    {
        List<Cartitem> GetAllCartitem();

        Cartitem AddToCart(Cartitem cartitem);

        Cartitem SearchUserAndProduct(int userid, int productId);
    }
}
