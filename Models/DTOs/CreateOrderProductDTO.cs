namespace Grimsby_and_Clee_Sells.Models.DTOs
{
    public class CreateOrderProductDTO
    {
        public int orderproducts_productid { get; set; }
        public int orderproducts_orderid { get; set; }
        public int orderproducts_quantity { get; set; }
    }
}
