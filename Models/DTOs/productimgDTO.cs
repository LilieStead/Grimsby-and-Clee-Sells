using Grimsby_and_Clee_Sells.Models.Domain;

namespace Grimsby_and_Clee_Sells.Models.DTOs
{
    public class productimgDTO
    {
        public int productimg_id {  get; set; }

        public byte[] productimg_img { get; set; }

        public int productimg_productid { get; set; }

        public byte[] productimg_thumbnail { get; set; }

        public Product Product { get; set; }
    }
}
