namespace Grimsby_and_Clee_Sells.Models.Domain
{
    public class Order
    {
        public int order_id { get; set; }

        public int order_userid { get; set; }
        public User User { get; set; }
        public string order_address { get; set; }

        public string order_postcode{ get; set;}

        public DateTime order_date { get; set; }

        public string? order_recipientname { get; set; }

        public string order_detail1 { get; set; }

        public string order_detail2 { get; set; }

        public string order_detail3 { get; set; }

        public int order_orderstatusid { get; set; }
        //public OrderStatus Orderstatus { get; set; }
    }
}
