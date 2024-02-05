using System.ComponentModel.DataAnnotations;

namespace Grimsby_and_Clee_Sells.Models.DTOs
{
    public class UpdateUserDTO
    {
        [Required]
        public int users_id { get; set; }
        [Required]
        public string users_username { get; set; }
        [Required]

        public string users_firstname { get; set; }
        [Required]

        public string users_lastname { get; set; }
        [Required]

        public string users_email { get; set; }
        [Required]

        public string users_phone { get; set; }
        [Required]

        public DateTime users_dob { get; set; }
        [Required]
        public double users_balance { get; set; }
    }
}
