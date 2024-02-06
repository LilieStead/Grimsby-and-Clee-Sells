using Grimsby_and_Clee_Sells.Data;
using Grimsby_and_Clee_Sells.Models.Domain;
using Grimsby_and_Clee_Sells.Models.DTOs;
using Grimsby_and_Clee_Sells.Repositories;
using Grimsby_and_Clee_Sells.UserSession;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Text;
using BCryptNet = BCrypt.Net.BCrypt;
using System.Text.RegularExpressions;

namespace Grimsby_and_Clee_Sells.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly GacsDbContext _context;
        private readonly IUserRepository _userRepository;
        private readonly DecodeJWT decodeJWT;

        public UserController(GacsDbContext context, IUserRepository userRepository, DecodeJWT decodeJWT)
        {
            _context = context;
            _userRepository = userRepository;
            this.decodeJWT = decodeJWT;
        }


        [HttpGet ("decodetoken")]
        public IActionResult DecodeToken () 
        {
            try
            {
                if (HttpContext.Session.TryGetValue("sessionid", out byte[] userbytes))
                {
                    string sessionid = Encoding.UTF8.GetString (userbytes);

                    if (Request.Cookies.TryGetValue("usercookie", out string usertoken))
                    {
                        var token = decodeJWT.DecodeToken(usertoken);
                        if (token.userid != null && token.username != null && token.firstname != null && token.lastname != null)
                        {
                            return Ok(new
                            {
                                Userid = token.userid,
                                Username = token.username,
                                Firstname = token.firstname,
                                Lastname = token.lastname
                            });
                        }
                        else
                        {
                            return BadRequest (new
                            {
                                Message = "Failed to decode token"
                            });
                        }
                    }
                    else
                    {
                        Logout ();
                        return BadRequest(new
                        {
                            Message = "Cookie not found"
                        });
                    }


                }
                else
                {
                    Response.Cookies.Delete("usercookie", new CookieOptions
                    {
                        HttpOnly = true,
                        SameSite = SameSiteMode.None,
                        Secure = true
                    });
                    Response.Cookies.Delete("usercookieexpiry", new CookieOptions
                    {
                        HttpOnly = false,
                        SameSite = SameSiteMode.None,
                        Secure = true
                    });
                    HttpContext.Session.Clear();

                    return Unauthorized(new
                    {
                        Message = "API has restarted token can not be decoded"
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "error deocding token",
                    Error = ex.Message
                });
            }

        }


        // to get all users
        [HttpGet]
        [Route("/getallusers")]
        public IActionResult Get()
        {
            try
            {
                var UserDM = _userRepository.GetAllUsers();
                if (UserDM.Count == 0)
                {
                    return NotFound();
                }
                return Ok(UserDM);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Could not connect to database", error = ex.Message });
            }

        }




        [HttpGet]
        [Route("/getuserbyid/{id:int}")]
        public IActionResult GetUserByID([FromRoute]int id)
        {
            try
            {
                var UserDM = _userRepository.GetUserByID(id);
                if (UserDM == null)
                {
                    return NotFound();
                }
                return Ok(UserDM);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Could not connect to database", error = ex.Message });
            }

        }



        [HttpPost]
        [Route("/signup")]
        public IActionResult UserSignUp([FromBody] CreateUserDTO createUserDTO) 
        {
            try
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
                        users_balance = 10000
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
                    // validate passowrd
                    if (!Regex.IsMatch(UserDM.users_password, @"[!@#$%^&*()_+{}\[\]:;<>,.?~\\/-]"))
                    {
                        return BadRequest();
                    }

                    // validate age
                    var minimumage = DateTime.Now.AddYears(-18);
                    if (UserDM.users_dob > minimumage)
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

                    foreach (var Users in usernameexists)
                    {
                        if (Users != null)
                        {
                            return Conflict(new { Message = "Username has been taken." });
                        }
                    }

                    foreach (var Phone in userphoneexists)
                    {
                        if (Phone != null)
                        {
                            return Conflict(new { Message = "Phone number is already in use." });
                        }
                    }

                    foreach (var Email in useremailexists)
                    {
                        if (Email != null)
                        {
                            return Conflict(new { Message = "Email Adress is already in use." });
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
                        users_balance = 10000
                    };


                    return CreatedAtAction("GetUserByID", new { id = CreateUsersDTO.users_id }, CreateUsersDTO);

                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Could not connect to database", error = ex.Message });
            }

        }


        [HttpGet]
        [Route("/login/{username}/{password}")]
        public IActionResult UserLogin([FromRoute]string username, string password)
        {
            try
            {
                Response.Cookies.Delete("usercookie", new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.None,
                    Secure = true
                });
                Response.Cookies.Delete("usercookieexpiry", new CookieOptions
                {
                    HttpOnly = false,
                    SameSite = SameSiteMode.None,
                    Secure = true
                });
                Response.Cookies.Delete("admincookie", new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.None,
                    Secure = true
                });
                Response.Cookies.Delete("admincookieexpiry", new CookieOptions
                {
                    HttpOnly = false,
                    SameSite = SameSiteMode.None,
                    Secure = true
                });

                HttpContext.Session.Clear();

                var usersDM = _userRepository.GetUserByUsername(username);
                if (usersDM == null)
                {
                    return NotFound(new { Message = "user does not exist" });
                }

                bool verifyPass = BCryptNet.EnhancedVerify(password, usersDM.users_password);
                if (!verifyPass)
                {
                    return Unauthorized(new { Message = "password is incorrect" });
                }

                var secret = new SecretKeyGen();
                var jwtgen = new JWTGen(secret);

                string jwttoken = jwtgen.Generate(usersDM.users_id.ToString(), usersDM.users_username.ToString(), usersDM.users_firstname.ToString(), usersDM.users_lastname.ToString());

                var exptime = DateTime.Now.AddDays(5);

                Response.Cookies.Append("usercookie", jwttoken, new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.None,
                    Secure = true,
                    Expires = exptime
                });

                Response.Cookies.Append("usercookieexpiry", exptime.ToString("R"), new CookieOptions
                {
                    HttpOnly = false,
                    SameSite = SameSiteMode.None,
                    Secure = true,
                    Expires = exptime
                });

                byte[] sessionidbytes = new byte[16];
                using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
                {
                    rng.GetBytes(sessionidbytes);
                };

                string sessionid = BitConverter.ToString(sessionidbytes).Replace("-", "");

                HttpContext.Session.SetString("sessionid", sessionid);

                var usersDTO = new UserDTO
                {
                    users_id = usersDM.users_id,
                    users_username = usersDM.users_username,
                    users_firstname = usersDM.users_firstname,
                    users_lastname = usersDM.users_lastname,
                    users_email = usersDM.users_email,
                    users_phone = usersDM.users_phone,
                    users_dob = usersDM.users_dob,
                    users_password = usersDM.users_password,
                };

                return Ok(new
                {
                    Token = jwttoken,
                    user = usersDTO,
                    Sessionid = sessionid
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Could not connect to database", error = ex.Message });
            }

        }



        [HttpPut]
        [Route("/updateuser/details")]
        public IActionResult UpdateUserDetails([FromBody] UpdateUserDTO updateUserDTO)
        {
            try
            {

                if (HttpContext.Session.TryGetValue("sessionid", out byte[] userbytes))
                {
                    string sessionid = Encoding.UTF8.GetString(userbytes);

                    if (Request.Cookies.TryGetValue("usercookie", out string usertoken))
                    {
                        if (updateUserDTO.users_id == null || updateUserDTO.users_username == null)
                        {
                            return Unauthorized(new { Message = "Please log in to update your details"});
                        }

                        var userDM = new User
                        {
                            users_id = updateUserDTO.users_id,
                            users_firstname = updateUserDTO.users_firstname,
                            users_lastname = updateUserDTO.users_lastname,
                            users_username = updateUserDTO.users_username,
                            users_dob = updateUserDTO.users_dob,
                            users_email = updateUserDTO.users_email,
                            users_phone = updateUserDTO.users_phone,
                            users_balance = updateUserDTO.users_balance,
                        };
                        if (userDM == null)
                        {
                            return NotFound(new { Message = "User contains no details" });
                        }
                        if (userDM.users_balance < 100)
                        {
                            return BadRequest(new { Message = "Please enter a balance larger than 100" });
                        }

                        var updateUser = _userRepository.UpdateUserDetails(userDM.users_id, userDM);
                        if (updateUser == null)
                        {
                            return BadRequest(new { Message = "Something went wrong when updating your details, please try again" });
                        }

                        return Ok(userDM);

                    }
                    else
                    {
                        Logout();
                        return BadRequest(new
                        {
                            Message = "Cookie not found"
                        });
                    }


                }
                else
                {
                    Response.Cookies.Delete("usercookie", new CookieOptions
                    {
                        HttpOnly = true,
                        SameSite = SameSiteMode.None,
                        Secure = true
                    });
                    Response.Cookies.Delete("usercookieexpiry", new CookieOptions
                    {
                        HttpOnly = false,
                        SameSite = SameSiteMode.None,
                        Secure = true
                    });
                    HttpContext.Session.Clear();

                    return Unauthorized(new
                    {
                        Message = "API has restarted token can not be decoded"
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Could not connect to database", error = ex.Message });
            }
        }


        [HttpGet]
        [Route("/Logout")]
        public IActionResult Logout() 
        {
            try
            {
                if (HttpContext.Session.TryGetValue("sessionid", out byte[] userbytes))
                {
                    string sessionid = Encoding.UTF8.GetString(userbytes);
                    Response.Cookies.Delete("usercookie", new CookieOptions
                    {
                        HttpOnly = true,
                        SameSite = SameSiteMode.None,
                        Secure = true
                    });
                    Response.Cookies.Delete("usercookieexpiry", new CookieOptions
                    {
                        HttpOnly = false,
                        SameSite = SameSiteMode.None,
                        Secure = true
                    });
                    HttpContext.Session.Clear();
                    return Ok(new { Message = "user logged out" });
                }

                else
                {
                    return BadRequest("user is not signed in");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Could not connect to database", error = ex.Message });
            }


        }
    }
    
}
