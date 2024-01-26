using Grimsby_and_Clee_Sells.Data;
using Grimsby_and_Clee_Sells.Models.Domain;
using Grimsby_and_Clee_Sells.Models.DTOs;
using Grimsby_and_Clee_Sells.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using System.Text;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;

namespace Grimsby_and_Clee_Sells.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly GacsDbContext _context;
        private readonly ICartitemRepository _cartitemRepository;
        private readonly IUserRepository _userRepository;
        private readonly IProductRepository _productRepository;
        public CartController(GacsDbContext context, ICartitemRepository cartitemRepository, IUserRepository userRepository, IProductRepository productRepository)
        {
            _context = context;
            _cartitemRepository = cartitemRepository;
            this._userRepository = userRepository;
            this._productRepository = productRepository;
        }
        [HttpGet("/GetAllCartItems")]
        public IActionResult GetAllCartItems() 
        {
            var cartitemDM = _cartitemRepository.GetAllCartitem();
            if (cartitemDM.Count == 0) 
            {
                return NotFound();
            }
            return Ok(cartitemDM);
        }

        [HttpPost]
        [Route("/AddToCart")]
        public IActionResult CreateCartItem([FromForm] AddCartitemDTO addCartitemDTO)
        {
            var userexits = _userRepository.GetUserByID(addCartitemDTO.cart_userid);

            if (userexits == null)
            {
                return Conflict(new { Message = "User not registered" });
            }

            var productexsits = _productRepository.GetProductById(addCartitemDTO.cart_productid);

            if (productexsits == null)
            {
                return Conflict(new { Message = "product is not a registered product" });
            }
            if (addCartitemDTO.cart_quantity <= 0)
            {
                return Conflict(new { Message = "you can not have a quantity of less than 1" });
            }

            var cartitemDM = new Cartitem
            {
                cart_userid = addCartitemDTO.cart_userid,
                cart_productid = addCartitemDTO.cart_productid,
                cart_quantity = addCartitemDTO.cart_quantity,
            };

            _cartitemRepository.AddToCart(cartitemDM);
            var cartitemDTO = new CartitemDTO
            {
                cart_userid = addCartitemDTO.cart_userid,
                cart_productid = addCartitemDTO.cart_productid,
                cart_quantity = addCartitemDTO.cart_quantity,
                User = cartitemDM.User,
                product = cartitemDM.product
            };
            return Ok(cartitemDTO);
        }
    }
    
}
