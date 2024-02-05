using Grimsby_and_Clee_Sells.Models.DTOs;

namespace Grimsby_and_Clee_Sells.Models.Domain
{
    public class Product
    {
        public int product_id { get; set; }

        public string product_name { get; set;}

        public string product_description { get; set;}

        public int product_category {  get; set;}

        public int product_userid { get; set;}

        public int product_status { get; set;}

        public double product_price { get; set;}
        public int product_sold { get; set; }

        public Category Category { get; set; }

        public User User { get; set; }

        public Status Status { get; set; }
    }
}
