using Grimsby_and_Clee_Sells.Models.Domain;

namespace Grimsby_and_Clee_Sells.Models.DTOs
{
    public class CreateOrderDTO
    {
        public int order_userid { get; set; }
        public string order_address { get; set; }

        public string order_postcode { get; set; }

        public string? order_recipientname { get; set; }

        public string order_detail1 { get; set; }

        public string order_detail2 { get; set; }

        public string order_detail3 { get; set; }
        public double total_price { get; set; }

        public List<int> productID { get; set; }
        public List<int> quantity { get; set; }
    }
}
