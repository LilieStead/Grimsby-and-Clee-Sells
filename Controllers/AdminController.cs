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

        [HttpGet]
        [Route("/getalladmins")]
        public IActionResult Get()
        {
            var AdminDM = _adminRepository.GetAllAdmins();
            if (AdminDM.Count == 0)
            {
                return NotFound();
            }
            return Ok(AdminDM);
        }
    }
}
