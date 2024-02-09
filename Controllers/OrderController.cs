using Grimsby_and_Clee_Sells.Data;
using Grimsby_and_Clee_Sells.Models.Domain;
using Grimsby_and_Clee_Sells.Models.DTOs;
using Grimsby_and_Clee_Sells.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.RegularExpressions;

namespace Grimsby_and_Clee_Sells.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly GacsDbContext _context;
        private readonly IProductRepository _ProductRepository;
        private readonly IUserRepository _UserRepository;
        private readonly ICategoryRepository _CategoryRepository;
        private readonly IOrderRepository _OrderRepository;
        private readonly ICartitemRepository _CartitemRepository;
        public OrderController(GacsDbContext context, IProductRepository ProductRepository, IUserRepository userRepository, ICategoryRepository categoryRepository, IOrderRepository orderRepository, ICartitemRepository cartitemRepository)
        {
            _context = context;
            _ProductRepository = ProductRepository;
            _UserRepository = userRepository;
            _CategoryRepository = categoryRepository;
            _OrderRepository = orderRepository;
            _CartitemRepository = cartitemRepository;
        }

        [HttpPost]
        [Route("/createorder")]
        public IActionResult CreateOrder([FromForm] CreateOrderDTO createOrderDTO)
        {
            try
            {
                if (HttpContext.Session.TryGetValue("sessionid", out byte[] userbytes))
                {
                    string sessionid = Encoding.UTF8.GetString(userbytes);

                    if (Request.Cookies.TryGetValue("usercookie", out string usertoken))
                    {
                        if (createOrderDTO.order_userid == null)
                        {
                            return Unauthorized(new { Message = "No user found, please try again"});
                        }
                        var product = createOrderDTO.productID;
                        // Validation for the card number
                        if (createOrderDTO.order_detail1.Length != 16)
                        {
                            return BadRequest(new { Message = "Please modify your card number to be 16 characters" });
                        }
                        // Validation for the expiry date
                        if (!Regex.IsMatch(createOrderDTO.order_detail2, @"^(0[1-9]|1[0-2])\/\d{2}$"))
                        {
                            return BadRequest(new { Message = "Expiry date does not match the format of: MM/YY" });
                        }
                        //validate cvv is there numbers
                        if (createOrderDTO.order_detail3.Length != 3)
                        {
                            return BadRequest(new { Message = "Ensure your CVV is only 3 numbers" });
                        }

                        //store all inputs form user in orderDM

                        var orderDM = new Order
                        {
                            order_userid = createOrderDTO.order_userid,
                            order_address = createOrderDTO.order_address,
                            order_date = DateTime.UtcNow,
                            // card number
                            order_detail1 = createOrderDTO.order_detail1,
                            // expiry
                            order_detail2 = createOrderDTO.order_detail2,
                            // cvv
                            order_detail3 = createOrderDTO.order_detail3,
                            order_postcode = createOrderDTO.order_postcode,
                            order_orderstatusid = 1,
                            order_recipientname = createOrderDTO.order_recipientname,
                        };
                        
                        //if its null return an error
                        if (orderDM == null)
                        {
                            return NotFound(new { Message = "Please select a valid item" });
                        }
                        // validate the user 
                        var userExists = _UserRepository.GetUserByID(orderDM.order_userid);
                        if (userExists == null)
                        {
                            return NotFound(new { Message = "No user found" });
                        }

                        // make sure the user has enough balance to make that order
                        if (userExists.users_balance < createOrderDTO.total_price)
                        {
                            return BadRequest(new { Message = "You do not have enough balance to purchase this item, please add more and try again" });
                        }

                        //if the name was not passed through use the users first and last name
                        if (orderDM.order_recipientname == null)
                        {
                            orderDM.order_recipientname = userExists.users_firstname + userExists.users_lastname;
                        }
                        else
                        {
                            orderDM.order_recipientname = createOrderDTO.order_recipientname;
                        }
                        // make sure the user has enough balance to make that order
                        if (userExists.users_balance < createOrderDTO.total_price)
                        {
                            return Conflict(new { Message = "You do not have enough money make an order, please add more to your balance and try again" });
                        }

                        // create an order
                        var createOrder = _OrderRepository.CreateOrder(orderDM);

                        List<OrderProduct> orderedProducts = new List<OrderProduct>();
                        
                        for (int i = 0; i < createOrderDTO.productID.Count; i++)
                        {
                            var existingProduct = createOrderDTO.productID[i];
                            var quantity = createOrderDTO.quantity[i];
                            if (existingProduct == null)
                            {
                                //error handling 
                                _OrderRepository.DeleteOrder(createOrder.order_id);
                                return NotFound(new { Message = "An error occurred when creating your order, please try again" });
                            }
                            var checkCart = _CartitemRepository.SearchUserAndProduct(userExists.users_id, existingProduct);
                            if (checkCart == null)
                            {// validation if a user doesnt have an item in a cart
                                _OrderRepository.DeleteOrder(createOrder.order_id);
                                return BadRequest(new { Message = "Please add this item to your cart before proceeding" });
                            }
                            
                            var ordered = new OrderProduct
                            {
                                orderproducts_orderid = createOrder.order_id,
                                orderproducts_quantity = quantity,
                                orderproducts_productid = existingProduct
                            };
                           
                            _OrderRepository.OrderProduct(ordered);
                            orderedProducts.Add(ordered);

                            _OrderRepository.ProductIsSold(existingProduct, quantity);
                            
                            _CartitemRepository.DeleteCartitem(userExists.users_id, ordered.orderproducts_productid);
                            
                            
                        }

                        var pay = _UserRepository.RemoveAmount(createOrderDTO.total_price, userExists.users_id);
                        if (pay == null)
                        {//add to list for the order
                            foreach(var products in createOrderDTO.productID)
                            {
                                _OrderRepository.RemoveOrderedProducts(createOrder.order_id, products);
                            }
                            //error handing
                            _OrderRepository.DeleteOrder(createOrder.order_id);
                            return BadRequest(new { Message = "Something went wrong, please try again" });
                        }
                        //get the otder that api just made
                        var productDTO = _OrderRepository.GetOrderByID(createOrder.order_id);

                        return Ok(new { Order = productDTO, OrderedProduct = orderedProducts});
                        
                    }
                    else
                    //delete cookie
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
                        return BadRequest(new
                        {
                            Message = "Cookie not found"
                        });
                    }

                }
                else
                {
                    // delete cookie if can not make order
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
            //if API gets to this point it means it could not reach the database
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Could not connect to database", error = ex.Message });
            }
            
        }

        [HttpGet]
        [Route ("/GetOrderByUserId/{userid:int}")]
        public IActionResult GetOrderByUserId([FromRoute] int userid)
        {
            try
            {
                //validate to see if the user has orders
                var productDM = _OrderRepository.GetOrderByUserId(userid);
                if (productDM.Count == 0)
                {
                    return NotFound(new {Message = "You have made no orders"});
                }

                return Ok(productDM);
            }
            catch (Exception ex)
            //if API gets to this point it means it could not reach the database
            {
                return BadRequest(new { Message = "Could not connect to database", error = ex.Message });
            }
        }
    }
}
