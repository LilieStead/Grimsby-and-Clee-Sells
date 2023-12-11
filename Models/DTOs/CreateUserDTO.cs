using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Grimsby_and_Clee_Sells.Models.DTOs
{
    public class CreateUserDTO
    {
        [Required]
        [MinLength(4, ErrorMessage = "username not is to desired length")]
        [MaxLength(20, ErrorMessage = "username not is to desired length")]
        public string users_username { get; set; }
        [Required]
        [MinLength(4, ErrorMessage = "first name  notis to desired length")]
        [MaxLength(20, ErrorMessage = "first name  notis to desired length")]
        public string users_firstname { get; set; }
        [Required]
        [MinLength(4, ErrorMessage = "last name not is to desired length")]
        [MaxLength(20, ErrorMessage = "last name not is to desired length")]
        public string users_lastname { get;set; }
        [Required]
        public string users_email { get; set; }
        [Required]
        [MinLength(11, ErrorMessage = "phone number not is to desired length")]
        [MaxLength(11, ErrorMessage = "phone number not is to desired length")]
        public string users_phone { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime users_dob { get; set; }
        [Required]
        [MinLength(10, ErrorMessage = "password is not to desired length")]
        [MaxLength(25, ErrorMessage = "password number not is to desired length")] 
        public string users_password { get; set; }
    }
}
