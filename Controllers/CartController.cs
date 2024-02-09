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
                    //if api get here it means the product id give doesnt exsit in the database
                    return Conflict(new { Message = "product is not a registered product" });
                }
                if (addCartitemDTO.cart_quantity <= 0)
                {
                    //if api gets here it means the qunity is lower than 0
                    return Conflict(new { Message = "you can not have a quantity of less than 1" });
                }
                if (addCartitemDTO.cart_userid == productexsits.product_userid)
                {
                    //if api gets here it means the user has tried to buy there own product 
                    return Unauthorized(new { Message = "You cannot add your own item to your cart" });
                }

                //validates that the product isnt already in the users cart 
                var exsits = _cartitemRepository.SearchUserAndProduct(addCartitemDTO.cart_userid, addCartitemDTO.cart_productid);
                if (exsits != null)
                {
                    //if api gets here it means that the product is in the users cart
                    return Conflict(new { Message = "The product is already in your cart" });
                }

                
                //used to make a nre cart item
                var cartitemDM = new Cartitem
                {
                    cart_userid = addCartitemDTO.cart_userid,
                    cart_productid = addCartitemDTO.cart_productid,
                    cart_quantity = addCartitemDTO.cart_quantity,
                };
                //validate that it isnt the users product already
                if (productexsits.product_userid == cartitemDM.cart_userid)
                {
                    //more validation to make sure the user doesnt by their own product 
                    return Conflict(new { Message = "You can not buy your own product" });
                }

                //add all fileds to database
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
            //if API gets to this point it means it could not reach the database
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
                //validate that the userid is assigned to a user in database
                var validateusers = _userRepository.GetUserByID(userid);
                if (validateusers == null)
                {
                    //if api gets here it means the user does not exist 
                    return Conflict(new { Message = "User not registered" });
                }
                // vaildate that the user has products in their cart 
                var CartDM = _cartitemRepository.GetCartitemByUserId(userid);
                if (CartDM.Count == 0)
                {
                    //if the API gets to this point it means that user doesnt have items in thier cart
                    return NotFound(new { Message = "You have no products in your cart" });
                }
                //output cart items 
                return Ok(CartDM);
            }
            //if API gets to this point it means it could not reach the database
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
                //validate the the user and product are in a cart 
                var vlaidateCart = _cartitemRepository.SearchUserAndProduct(deleteCartItemDTO.cart_userid, deleteCartItemDTO.cart_productid);
                if (vlaidateCart == null)
                {
                    //if both in the same able do not exist send here
                    return NotFound(new { Message = "This product does not exist in your cart" });
                }
                try
                {
                    //try and remove
                    var removeProduct = _cartitemRepository.DeleteCartitem(deleteCartItemDTO.cart_userid, deleteCartItemDTO.cart_productid);
                    return Ok(removeProduct);

                }
                // if API cant remove the data it mean it could not connect to the database
                catch
                {
                    return BadRequest(new { Message = "Could not connect to database" });
                }
            }
            catch (Exception ex)
            {
                //if API gets to this point it means it could not reach the database
                return BadRequest(new { Message = "Could not connect to database", error = ex.Message });
            }



        }

        [HttpPut]
        [Route("EditCartItemQuantity")]
        public IActionResult EditCartItemQuantity([FromForm] UpdateCartItemDTO updateCartItemDTO)
        {
            try
            {
                //validate the the user and product are in a cart 
                var validateCart = _cartitemRepository.SearchUserAndProduct(updateCartItemDTO.cart_userid, updateCartItemDTO.cart_productid);
                if (updateCartItemDTO.cart_quantity <= 0)
                {
                    //quantity is 0 send this to the user 
                    return Conflict(new { Message = "you can not have a quantity of less than 1" });
                }
                //if vaildate cart is null it means that the cart item in not in the database 
                if (validateCart == null)
                {
                    return NotFound(new { Message = "This product does not exist in your cart" });
                }
                //update the cart 
                var update = _cartitemRepository.UpdateCartItem( updateCartItemDTO.cart_quantity, validateCart.cart_userid, validateCart.cart_productid);
                return Ok(update);

            }
            //if API gets to this point it means it could not reach the database
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Could not connect to database", error = ex.Message });
            }
        }
    }
    
}
