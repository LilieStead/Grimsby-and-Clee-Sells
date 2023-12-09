using Grimsby_and_Clee_Sells.Data;
using Grimsby_and_Clee_Sells.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet]
        public IActionResult Get() { 
            var UserDM = _userRepository.GetAllUsers();
            if (UserDM == null)
            {
                return NotFound();
            }
            return Ok(UserDM);
        }
    }
}
