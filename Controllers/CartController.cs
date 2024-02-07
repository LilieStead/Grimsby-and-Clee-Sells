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
            //doesnt need try and catch validation as it wont be used and is only for testing
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
            try
            {
                //Validates that the user exits 
                var userexits = _userRepository.GetUserByID(addCartitemDTO.cart_userid);

                if (userexits == null)
                {
                    return Conflict(new { Message = "User not registered" });
                }
                //vlaidates that the product exits
                var productexsits = _productRepository.GetProductById(addCartitemDTO.cart_productid);

                if (productexsits == null)
                {
                    return Conflict(new { Message = "product is not a registered product" });
                }
                if (addCartitemDTO.cart_quantity <= 0)
                {
                    return Conflict(new { Message = "you can not have a quantity of less than 1" });
                }
                if (addCartitemDTO.cart_userid == productexsits.product_userid)
                {
                    return Unauthorized(new { Message = "You cannot add your own item to your cart" });
                }

                //validates that the product isnt already in the users cart 
                var exsits = _cartitemRepository.SearchUserAndProduct(addCartitemDTO.cart_userid, addCartitemDTO.cart_productid);
                if (exsits != null)
                {
                    return Conflict(new { Message = "The product is already in your cart" });
                }

                

                var cartitemDM = new Cartitem
                {
                    cart_userid = addCartitemDTO.cart_userid,
                    cart_productid = addCartitemDTO.cart_productid,
                    cart_quantity = addCartitemDTO.cart_quantity,
                };
                //validate that it isnt the users product already
                if (productexsits.product_userid == cartitemDM.cart_userid)
                {
                    return Conflict(new { Message = "You can not buy your own product" });
                }

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

            catch (Exception ex)
            {
                return BadRequest(new { Message = "Could not connect to database", error = ex.Message });
            }

        }

        [HttpGet]
        [Route("/SearchByUserId/{userid:int}")]
        public IActionResult SeachCartItemByUserId ([FromRoute] int userid)
        {
            try
            {
                var validateusers = _userRepository.GetUserByID(userid);
                if (validateusers == null)
                {
                    return Conflict(new { Message = "User not registered" });
                }

                var CartDM = _cartitemRepository.GetCartitemByUserId(userid);
                if (CartDM.Count == 0)
                {

                    return NotFound(new { Message = "You have no products in your cart" });
                }

                return Ok(CartDM);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Could not connect to database", error = ex.Message });
            }


        }

        [HttpDelete]
        [Route("DeleteUsersCart")]
        public IActionResult DeleteUsersCart([FromForm] DeleteCartItemDTO deleteCartItemDTO)
        {
            try
            {
                var vlaidateCart = _cartitemRepository.SearchUserAndProduct(deleteCartItemDTO.cart_userid, deleteCartItemDTO.cart_productid);
                if (vlaidateCart == null)
                {
                    return NotFound(new { Message = "This product does not exist in your cart" });
                }
                try
                {
                    var removeProduct = _cartitemRepository.DeleteCartitem(deleteCartItemDTO.cart_userid, deleteCartItemDTO.cart_productid);
                    return Ok(removeProduct);

                }
                catch
                {
                    return BadRequest(new { Message = "Could not connect to database" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Could not connect to database", error = ex.Message });
            }



        }

        [HttpPut]
        [Route("EditCartItemQuantity")]
        public IActionResult EditCartItemQuantity([FromForm] UpdateCartItemDTO updateCartItemDTO)
        {
            try
            {
                var validateCart = _cartitemRepository.SearchUserAndProduct(updateCartItemDTO.cart_userid, updateCartItemDTO.cart_productid);
                if (updateCartItemDTO.cart_quantity <= 0)
                {
                    return Conflict(new { Message = "you can not have a quantity of less than 1" });
                }
                if (validateCart == null)
                {
                    return NotFound(new { Message = "This product does not exist in your cart" });
                }
                var update = _cartitemRepository.UpdateCartItem( updateCartItemDTO.cart_quantity, validateCart.cart_userid, validateCart.cart_productid);
                return Ok(update);

            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Could not connect to database", error = ex.Message });
            }
        }
    }
    
}
