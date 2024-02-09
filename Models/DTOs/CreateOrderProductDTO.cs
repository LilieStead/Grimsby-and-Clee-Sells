using System.ComponentModel.DataAnnotations;

namespace Grimsby_and_Clee_Sells.Models.DTOs
{
    public class CreateOrderProductDTO
    {
        [Required]
        public int orderproducts_productid { get; set; }
        [Required]
        public int orderproducts_orderid { get; set; }
        [Required]
        public int orderproducts_quantity { get; set; }
    }
}
