using System.ComponentModel.DataAnnotations;

namespace Grimsby_and_Clee_Sells.Models.DTOs
{
    public class UpdateProductImgDTO
    {
        [Required]
        public List<IFormFile> productimg_img { get; set; }
        [Required]
        public int productimg_productid { get; set; }
    }
}
