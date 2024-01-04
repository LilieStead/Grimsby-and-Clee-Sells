using Grimsby_and_Clee_Sells.Data;
using Grimsby_and_Clee_Sells.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Grimsby_and_Clee_Sells.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
  
        private readonly GacsDbContext _context;
        private readonly IProductRepository _ProductRepository;
        public ProductController(GacsDbContext context, IProductRepository ProductRepository)
        {
            _context = context;
            _ProductRepository = ProductRepository;
        }
        [HttpGet ("GetAllProducts")]
        public IActionResult GetAllProduct() {
            var productDM = _ProductRepository.GetAllProduct();
            if (productDM.Count == 0)
            {
                return NotFound();
            }
            return Ok(productDM);
        }
    }

}
