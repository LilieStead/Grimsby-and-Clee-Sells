using Grimsby_and_Clee_Sells.Data;
using Grimsby_and_Clee_Sells.Models.Domain;
using Grimsby_and_Clee_Sells.Models.DTOs;
using Grimsby_and_Clee_Sells.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using BCryptNet = BCrypt.Net.BCrypt;

namespace Grimsby_and_Clee_Sells.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly GacsDbContext _context;
        private readonly IUserRepository _userRepository;

        public UserController(GacsDbContext context, IUserRepository userRepository)
        {
            _context = context;
            _userRepository = userRepository;
        }


        // to get all users
        [HttpGet]
        [Route("/getallusers")]
        public IActionResult Get() { 
            var UserDM = _userRepository.GetAllUsers();
            if (UserDM == null)
            {
                return NotFound();
            }
            return Ok(UserDM);
        }



        [HttpPost]
        [Route("/signup")]
        public IActionResult UserSignUp([FromBody] CreateUserDTO createUserDTO) 
        {
            if (ModelState.IsValid)
            {
                var UserDM = new User
                {
                    users_username = createUserDTO.users_username,
                    users_firstname = createUserDTO.users_firstname,
                    users_lastname = createUserDTO.users_lastname,
                    users_email = createUserDTO.users_email,
                    users_phone = createUserDTO.users_phone,
                    users_dob = createUserDTO.users_dob,
                    users_password = createUserDTO.users_password,

                };

                // validate format of phone number 
                if (!System.Text.RegularExpressions.Regex.IsMatch(UserDM.users_phone, @"^07\d{9}$"))
                {
                    return BadRequest();
                }

                // validate format of email
                if (!System.Text.RegularExpressions.Regex.IsMatch(UserDM.users_email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
                {
                    return BadRequest();
                }


                // hash password
                string passwordhash = BCryptNet.EnhancedHashPassword(UserDM.users_password);
                UserDM.users_password = passwordhash;


                //validation for information that already exists

                var usernameexists = _userRepository.GetAllUsers().Where(x => x.users_username == UserDM.users_username);
                var useremailexists = _userRepository.GetAllUsers().Where(x => x.users_email == UserDM.users_email);
                var userphoneexists = _userRepository.GetAllUsers().Where(x => x.users_phone == UserDM.users_phone);

                foreach ( var Users in usernameexists )
                {
                    if (Users != null )
                    {
                        return Conflict("Username has been taken.");
                    }
                }

                foreach ( var Phone in userphoneexists)
                {
                    if (Phone != null)
                    {
                        return Conflict("Phone number is already in use.");
                    }
                }

                foreach (var Email in useremailexists)
                {
                    if (Email != null)
                    {
                        return Conflict("Email Adress is already in use.");
                    }
                }
                _userRepository.UserSignUp(UserDM);
                var CreateUsersDTO = new UserDTO
                {
                    users_username = UserDM.users_username,
                    users_firstname = UserDM.users_firstname,
                    users_lastname = UserDM.users_lastname,
                    users_email = UserDM.users_email,
                    users_phone = UserDM.users_phone,
                    users_dob = UserDM.users_dob,
                    users_password = UserDM.users_password,
                };


                return Ok();

            }
            else
            {
                return BadRequest();
            }
        }
    }
}
