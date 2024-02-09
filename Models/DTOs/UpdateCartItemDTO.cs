using System.ComponentModel.DataAnnotations;

namespace Grimsby_and_Clee_Sells.Models.DTOs
{
    public class UpdateCartItemDTO
    {
        [Required]
        public int cart_userid { get; set; }
        [Required]
        public int cart_productid { get; set; }
        [Required]
        public int cart_quantity { get; set; }


}
}
