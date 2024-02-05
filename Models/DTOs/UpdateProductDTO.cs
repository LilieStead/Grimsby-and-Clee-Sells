using System.ComponentModel.DataAnnotations;

namespace Grimsby_and_Clee_Sells.Models.DTOs
{
    public class UpdateProductDTO
    {
        [Required]
        public int product_id { get; set; }
        [Required]
        [MaxLength(50, ErrorMessage = "productname is not to desired length")]
        [MinLength(10, ErrorMessage = "productname is not to desired length")]
        public string product_name { get; set; }
        [Required]
        [MaxLength(200, ErrorMessage = "productdescription is not to desired length")]
        [MinLength(25, ErrorMessage = "productname is not to desired length")]
        public string product_description { get; set; }
        [Required]
        public int product_category { get; set; }
        [Required]
        public int product_userid { get; set; }
        public int? product_status { get; set; }
        [Required]
        public double product_price { get; set; }
    }
}
