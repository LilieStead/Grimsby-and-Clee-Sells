namespace Grimsby_and_Clee_Sells.Models.Domain
{
    public class User
    {
        public int users_id { get; set; }

        public string users_username { get; set; }

        public string users_firstname { get; set; }

        public string users_lastname { get;set; }

        public string users_email { get; set;}

        public string users_phone { get; set; }

        public DateTime users_dob { get; set; }

        public string users_password { get; set; }
        public double users_balance { get; set; }
    }
}
