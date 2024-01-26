using Grimsby_and_Clee_Sells.Models.Domain;

namespace Grimsby_and_Clee_Sells.Models.DTOs
{
    public class CartitemDTO
    {
        public int cart_userid { get; set; }

        public int cart_productid { get; set; }

        public int cart_quantity { get; set; }

        public User User { get; set; }
        public Product product { get; set; }
    }
}
