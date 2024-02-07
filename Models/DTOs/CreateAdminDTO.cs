using System.ComponentModel.DataAnnotations;

namespace Grimsby_and_Clee_Sells.Models.DTOs
{
    public class CreateAdminDTO
    {
        [Required]
        public string admin_username { get; set; }
        [Required]
        public string admin_firstname { get; set; }
        [Required]
        public string admin_lastname { get; set; }
        [Required]
        public string admin_email { get; set; }
        [Required]
        public string admin_phone { get; set; }
        [Required]
        public DateTime admin_dob { get; set; }
        [Required]
        public string admin_password { get; set; }
    }
}
