using Grimsby_and_Clee_Sells.Models.Domain;
using System.ComponentModel.DataAnnotations;

namespace Grimsby_and_Clee_Sells.Models.DTOs
{
    public class CreateOrderDTO
    {
        [Required]
        public int order_userid { get; set; }
        [Required]
        public string order_address { get; set; }
        [Required]

        public string order_postcode { get; set; }

        public string? order_recipientname { get; set; }
        [Required]
        public string order_detail1 { get; set; }
        [Required]
        public string order_detail2 { get; set; }
        [Required]
        public string order_detail3 { get; set; }
        [Required]
        public double total_price { get; set; }
        [Required]
        public List<int> productID { get; set; }
        public List<int> quantity { get; set; }
    }
}
