using Grimsby_and_Clee_Sells.Data;
using Grimsby_and_Clee_Sells.Models.Domain;
using Grimsby_and_Clee_Sells.Models.DTOs;
using Grimsby_and_Clee_Sells.Repositories;
using Grimsby_and_Clee_Sells.UserSession;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using BCryptNet = BCrypt.Net.BCrypt;

namespace Grimsby_and_Clee_Sells.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly GacsDbContext _context;
        private readonly IAdminRepository _adminRepository;
        private readonly DecodeJWT decodeJWT;

        public AdminController(GacsDbContext context, IAdminRepository adminRepository, DecodeJWT decodeJWT)
        {
            _context = context;
            _adminRepository = adminRepository;
            this.decodeJWT = decodeJWT;
        }

        [HttpGet("decodeadmin")]
        public IActionResult DecodeAdmin()
        {
            try
            {
                try
                {
                    if (HttpContext.Session.TryGetValue("sessionid", out byte[] userbytes))
                    {
                        string sessionid = Encoding.UTF8.GetString(userbytes);

                        if (Request.Cookies.TryGetValue("admincookie", out string usertoken))
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
                                return BadRequest(new
                                {
                                    Message = "Failed to decode token"
                                });
                            }
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
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Could not connect to database", error = ex.Message });
            }


        }

        [HttpGet]
        [Route("/getalladmins")]
        public IActionResult Get()
        {
            try
            {
                var AdminDM = _adminRepository.GetAllAdmins();
                if (AdminDM.Count == 0)
                {
                    return NotFound();
                }
                return Ok(AdminDM);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Could not connect to database", error = ex.Message });
            }

        }

        [HttpGet]
        [Route("/getadminbyid/{id:int}")]
        public IActionResult GetAdminByID([FromRoute] int id)
        {
            try
            {
                var adminDM = _adminRepository.GetAdminByID(id);
                if (adminDM == null)
                {
                    return NotFound(new { Message = "Admin not found" });
                }
                return Ok(adminDM);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Could not connect to database", error = ex.Message });
            }
        }

        [HttpPost]
        [Route("/CreateAdmin")]
        public IActionResult CreateAdmin([FromForm] CreateAdminDTO createAdminDTO)
        {
            try
            {
                var adminExists = _adminRepository.GetAdminByUsername(createAdminDTO.admin_username);
                if (adminExists != null)
                {
                    return BadRequest(new { Message = "This username is already taken" });
                }
                // validate format of phone number 
                if (!System.Text.RegularExpressions.Regex.IsMatch(createAdminDTO.admin_phone, @"^07\d{9}$"))
                {
                    return BadRequest(new { Message = "Phone number does not start with: 07"});
                }
                // validate format of email
                if (!System.Text.RegularExpressions.Regex.IsMatch(createAdminDTO.admin_email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
                {
                    return BadRequest(new { Message = "Email address is not of the correct format"});
                }
                // validate passowrd
                if (!System.Text.RegularExpressions.Regex.IsMatch(createAdminDTO.admin_password, @"[!@#$%^&*()_+{}\[\]:;<>,.?~\\/-]"))
                {
                    return BadRequest(new { Message = "Password is not of the correct format, please make sure to include special characters like @ or !"});
                }

                string passwordHash = BCryptNet.EnhancedHashPassword(createAdminDTO.admin_password);
                if (passwordHash == null)
                {
                    return BadRequest(new { Message = "Please enter a password" });
                }
                createAdminDTO.admin_password = passwordHash;
                var checkPhone = _adminRepository.GetAdminByPhone(createAdminDTO.admin_phone);
                if (checkPhone != null)
                {
                    return Conflict(new { Message = "Phone number already exists in our records" });
                }
                var checkEmail = _adminRepository.GetAdminByEmail(createAdminDTO.admin_email);
                if (checkEmail != null)
                {
                    return Conflict(new { Message = "Email address already exists in our records" }); 
                }

                var adminDM = new Admin
                {
                    admin_firstname = createAdminDTO.admin_firstname,
                    admin_lastname = createAdminDTO.admin_lastname,
                    admin_email = createAdminDTO.admin_email,
                    admin_password = createAdminDTO.admin_password,
                    admin_dob = createAdminDTO.admin_dob,
                    admin_phone = createAdminDTO.admin_phone,
                    admin_username = createAdminDTO.admin_username,
                };
                var createAdmin = _adminRepository.CreateAdmin(adminDM);
                var adminDTO = new AdminDTO
                {
                    admin_id = createAdmin.admin_id,
                    admin_firstname = adminDM.admin_firstname,
                    admin_lastname = adminDM.admin_lastname,
                    admin_email = adminDM.admin_email,
                    admin_password = adminDM.admin_password,
                    admin_dob = adminDM.admin_dob,
                    admin_phone = adminDM.admin_phone,
                    admin_username = adminDM.admin_username,
                };
                return CreatedAtAction("GetAdminByID", new { id = adminDTO.admin_id }, adminDTO);
            }
            catch(Exception ex)
            {
                return BadRequest(new { Message = "Could not connect to database", error = ex.Message });
            }
        }

        [HttpGet]
        [Route("/adminlogin/{username}/{password}")]

        public IActionResult AdminLogin([FromRoute] string username, string password)
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
                var adminDM = _adminRepository.GetAdminByUsername(username);
                if (adminDM == null)
                {
                    return NotFound(new { Message = "Could not find user" });
                }

                bool verifyPass = BCryptNet.EnhancedVerify(password, adminDM.admin_password);
                if (!verifyPass)
                {
                    return Unauthorized(new { Message = "password is incorrect" });
                }

                var secret = new SecretKeyGen();
                var jwtgen = new JWTGen(secret);

                string jwttoken = jwtgen.Generate(adminDM.admin_id.ToString(), adminDM.admin_username.ToString(), adminDM.admin_firstname.ToString(), adminDM.admin_lastname.ToString());

                var exptime = DateTime.Now.AddDays(5);

                Response.Cookies.Append("admincookie", jwttoken, new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.None,
                    Secure = true,
                    Expires = exptime
                });

                Response.Cookies.Append("admincookieexpiry", exptime.ToString("R"), new CookieOptions
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

                var adminDTO = new AdminDTO
                {
                    admin_id = adminDM.admin_id,
                    admin_username = adminDM.admin_username,
                    admin_firstname = adminDM.admin_firstname,
                    admin_lastname = adminDM.admin_lastname,
                    admin_email = adminDM.admin_email,
                    admin_phone = adminDM.admin_phone,
                    admin_dob = adminDM.admin_dob,
                    admin_password = adminDM.admin_password,
                };

                return Ok(new
                {
                    Token = jwttoken,
                    admin = adminDTO,
                    Sessionid = sessionid
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Could not connect to database", error = ex.Message });
            }


        }


        [HttpGet]
        [Route("/adminsearch/users")]
        public async Task<IActionResult> GetUsersBySearch([FromQuery] string users)
        {
            try
            {
                try
                {
                    var adminDM = await _adminRepository.GetUsersBySearch(users);
                    if (adminDM.Count == 0)
                    {
                        return NotFound(new { Message = "No users were found with this name" });
                    }
                    return Ok(adminDM);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { Message = "An error has occurred", Error = ex.Message });
                }
            }
            catch(Exception ex)
            {
                return BadRequest(new { Message = "Could not connect to database", error = ex.Message });
            }
            
        }

        [HttpGet]
        [Route("/AdminLogout")]
        public IActionResult Logout()
        {
            try
            {
                if (HttpContext.Session.TryGetValue("sessionid", out byte[] userbytes))
                {
                    string sessionid = Encoding.UTF8.GetString(userbytes);
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
