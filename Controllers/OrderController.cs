﻿using Grimsby_and_Clee_Sells.Data;
using Grimsby_and_Clee_Sells.Models.Domain;
using Grimsby_and_Clee_Sells.Models.DTOs;
using Grimsby_and_Clee_Sells.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;

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
                        if (orderDM == null)
                        {
                            return NotFound(new { Message = "Please select a valid item" });
                        }
                        var userExists = _UserRepository.GetUserByID(orderDM.order_userid);
                        if (userExists == null)
                        {
                            return NotFound(new { Message = "No user found" });
                        }
                        if (orderDM.order_recipientname == null)
                        {
                            orderDM.order_recipientname = userExists.users_firstname + userExists.users_lastname;
                        }
                        else
                        {
                            orderDM.order_recipientname = createOrderDTO.order_recipientname;
                        }

                        var createOrder = _OrderRepository.CreateOrder(orderDM);


                        List<OrderProduct> orderedProducts = new List<OrderProduct>();
                        for (int i = 0; i < createOrderDTO.productID.Count; i++)
                        {
                            var existingProduct = createOrderDTO.productID[i];
                            var quantity = createOrderDTO.quantity[i];
                            if (existingProduct == null)
                            {
                                return NotFound(new { Message = "An error occurred when creating your order, please try again" });
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

                            _CartitemRepository.DeleteCartitem(userExists.users_id, existingProduct);
                        }

                        var pay = _UserRepository.RemoveAmount(createOrderDTO.total_price, userExists.users_id);
                        if (pay == null)
                        {
                            return BadRequest(new { Message = "Something went wrong, please try again" });
                        }

                        var productDTO = new Order
                        {
                            order_id = createOrder.order_id,
                            order_address = createOrderDTO.order_address,
                            order_date = createOrder.order_date,
                            order_detail1 = createOrderDTO.order_detail1,
                            order_detail2 = createOrderDTO.order_detail2,
                            order_detail3 = createOrderDTO.order_detail3,
                            order_postcode = createOrderDTO.order_postcode,
                            order_orderstatusid = createOrder.order_orderstatusid,
                            order_userid = createOrderDTO.order_userid,
                            order_recipientname = createOrderDTO.order_recipientname,
                        };

                        return Ok(new { Order = productDTO, OrderedProduct = orderedProducts});




                        
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
    }
}
