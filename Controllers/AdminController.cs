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
                    return NotFound();
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
