using System.ComponentModel.DataAnnotations;

namespace Grimsby_and_Clee_Sells.Models.DTOs
{
    public class CreateProductDTO
    {
        [Required]
        public string product_name { get; set; }
        [Required]
        public string product_description { get; set; }
        [Required]
        public int product_category { get; set; }
        [Required]
        public int product_userid { get; set; }
        [Required]
        public int product_status { get; set; }
        [Required]
        public double product_price { get; set; }
    }
}
