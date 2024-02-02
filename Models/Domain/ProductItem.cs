namespace Grimsby_and_Clee_Sells.Models.Domain
{
    public class ProductItem
    {
        public Product Product { get; set; }
        public int orderproducts_productid {  get; set; }

        public Order Order { get; set; }
        public int orderproducts_orderid { get; set; }
    }
}
