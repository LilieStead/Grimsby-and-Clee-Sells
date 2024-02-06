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
using System.Net.WebSockets;

namespace Grimsby_and_Clee_Sells.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {

        private readonly GacsDbContext _context;
        private readonly IProductRepository _ProductRepository;
        private readonly IUserRepository _UserRepository;
        private readonly ICategoryRepository _CategoryRepository;
        public ProductController(GacsDbContext context, IProductRepository ProductRepository, IUserRepository userRepository, ICategoryRepository categoryRepository)
        {
            _context = context;
            _ProductRepository = ProductRepository;
            _UserRepository = userRepository;
            _CategoryRepository = categoryRepository;
        }
        [HttpGet("GetAllProducts")]
        public IActionResult GetAllProduct() {
            var productDM = _ProductRepository.GetAllProduct();
            if (productDM.Count == 0)
            {
                return NotFound();
            }
            return Ok(productDM);
        }


        [HttpPost]
        [Route("/CreateProduct")]
        public IActionResult CreateProduct([FromForm] CreateProductDTO createProductDTO)
        {
            try
            {
                int status = 1;


                if (ModelState.IsValid)
                {
                    var ProductDM = new Product
                    {
                        product_name = createProductDTO.product_name,
                        product_description = createProductDTO.product_description,
                        product_category = createProductDTO.product_category,
                        product_userid = createProductDTO.product_userid,
                        product_status = status,
                        product_price = createProductDTO.product_price,
                        product_sold = 0
                    };

                    var userexits = _UserRepository.GetAllUsers().Where(x => x.users_id == ProductDM.product_userid);

                    if (!userexits.Any())
                    {
                        return Conflict(new { Message = "User not registered" });
                    }

                    var catgoryexsits = _CategoryRepository.GetAllCategory().Where(x => x.category_id == ProductDM.product_category);

                    if (!catgoryexsits.Any())
                    {
                        return Conflict(new { Message = "category does not exisit" });
                    }




                    const string priceformate = @"^\d+(\.\d{1,2})?$";

                    string test = ProductDM.product_price.ToString();

                    if (!System.Text.RegularExpressions.Regex.IsMatch(test, priceformate))
                    {
                        return BadRequest();
                    }


                    _ProductRepository.CreateProduct(ProductDM);
                    var CreateProductDTO = new ProductDTO
                    {
                        product_id = ProductDM.product_id,
                        product_name = ProductDM.product_name,
                        product_description = ProductDM.product_description,
                        product_category = ProductDM.product_category,
                        product_userid = ProductDM.product_userid,
                        product_status = ProductDM.product_status,
                        product_price = ProductDM.product_price,
                        product_sold = 0
                    };

                    return CreatedAtAction("GetProductById", new { id = CreateProductDTO.product_id }, CreateProductDTO);

                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Could not connect to database", error = ex.Message });
            }

        }

        [HttpPut]
        [Route("/updateproduct")]
        public IActionResult UpdateProduct([FromForm] UpdateProductDTO updateProductDTO)
        {

            if (HttpContext.Session.TryGetValue("sessionid", out byte[] userbytes))
            {
                string sessionid = Encoding.UTF8.GetString(userbytes);

                if (Request.Cookies.TryGetValue("usercookie", out string usertoken))
                {
                    if (updateProductDTO.product_id == null)
                    {
                        return Conflict(new { Message = "This is not a valid item, please try again" });
                    
                    }
                    var productDM = new Product
                    {
                        product_id = updateProductDTO.product_id,
                        product_name = updateProductDTO.product_name,
                        product_description = updateProductDTO.product_description,
                        product_category = updateProductDTO.product_category,
                        product_price = updateProductDTO.product_price,
                        product_status = 1,
                        product_userid = updateProductDTO.product_userid,
                        
                    };
                    if (productDM == null)
                    {
                        return NotFound(new { Message = "No item found, please try again" });
                    }
                    var userExists = _UserRepository.GetUserByID(productDM.product_userid);
                    var productExists = _ProductRepository.GetProductById(productDM.product_id);
                    if (userExists == null)
                    {
                        return NotFound(new { Message = "No user found" });
                    }
                    if (userExists.users_id != productExists.product_userid)
                    {
                        return Unauthorized(new { Message = "You are attempting to modify an item that is not yours"});
                    }

                    var updateProduct = _ProductRepository.UpdateProduct(productDM.product_id, productDM);
                    if (updateProduct == null)
                    {
                        return BadRequest(new { Message = "Something went wrong when updating your product, please try again" });
                    }

                    return Ok(productDM);

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

        [HttpGet]
        [Route("/getproductbyid/{id:int}")]
        public IActionResult GetProductById([FromRoute] int id)
        {
            try
            {
                var ProductDM = _ProductRepository.GetProductById(id);

                if (ProductDM == null)
                {
                    return NotFound();
                }

                return Ok(ProductDM);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Could not connect to database", error = ex.Message });
            }

        }


        [HttpPost]
        [Route("/createproductimg")]
        public async Task<IActionResult> CreateProductImg([FromForm]CreateProductimgDTO createProductimgDTO)
        {
            try
            {
                List<productimgDTO> productimgDTOs = new List<productimgDTO>();
                foreach (var img in createProductimgDTO.productimg_img)
                {
                    var ProductDM = new Productimg
                    {
                        productimg_img = await ConvetImgToByte(img),
                        productimg_productid = createProductimgDTO.productimg_productid
                    };
                    if (ProductDM == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        var stream = img.OpenReadStream();
                        var thumbnail = CreateThumbnail(660, 500, stream);
                        ProductDM.productimg_thumbnail = thumbnail;
                        await _ProductRepository.CreateProductImg(ProductDM);
                        var productDTO = new productimgDTO
                        {
                            productimg_id = ProductDM.productimg_id,
                            productimg_img = ProductDM.productimg_img,
                            productimg_productid = ProductDM.productimg_productid,
                            productimg_thumbnail = ProductDM.productimg_thumbnail
                        };

                        productimgDTOs.Add(productDTO);
                    }
                }
                return Ok(productimgDTOs);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Could not connect to database", error = ex.Message });
            }

        }


        [HttpPut]
        [Route("/updateproductimg/{index:int}")]
        public async Task<IActionResult> UpdateProductImg([FromForm] UpdateProductImgDTO updateProductImgDTO, [FromRoute] int index)
        {
            try
            {

                if (HttpContext.Session.TryGetValue("sessionid", out byte[] userbytes))
                {
                    string sessionid = Encoding.UTF8.GetString(userbytes);

                    if (Request.Cookies.TryGetValue("usercookie", out string usertoken))
                    {
                        var getProduct = _ProductRepository.GetProductById(updateProductImgDTO.productimg_productid);
                        if (getProduct == null)
                        {
                            return NotFound(new { Message = "No item found, please try again" });
                        }
                        if (getProduct.product_id != updateProductImgDTO.productimg_productid)
                        {
                            return Unauthorized(new { Message = "Item's identification does not match this image" });
                        }



                        List<productimgDTO> productimgs = new List<productimgDTO>();
                        foreach(var image in updateProductImgDTO.productimg_img)
                        {
                            if (image == null || updateProductImgDTO.productimg_img == null)
                            {
                                return NoContent();
                            }
                            var productDM = new Productimg
                            {
                                productimg_img = await ConvetImgToByte(image),
                                productimg_productid = updateProductImgDTO.productimg_productid
                            };
                            if (productDM.productimg_img == null)
                            {
                                return NotFound(new { Message = "No image was sent, please try again" });
                            }

                            var findImage = await _ProductRepository.GetProductImgsById(updateProductImgDTO.productimg_productid, index);
                            if (findImage == null)
                            {
                                return BadRequest(new { Message = "Cannot edit image due to the image not existing" });
                            }
                            var openStream = image.OpenReadStream();
                            var thumbnail = CreateThumbnail(660, 500, openStream);
                            productDM.productimg_thumbnail = thumbnail;

                            await _ProductRepository.UpdateImage(getProduct.product_id, productDM, index);
                            var productDTO = new productimgDTO
                            {
                                productimg_id = findImage.productimg_id,
                                productimg_img = productDM.productimg_img,
                                productimg_productid = productDM.productimg_productid,
                                productimg_thumbnail = thumbnail,
                            };
                            productimgs.Add(productDTO);
                        }
                        return Accepted(productimgs);
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

        private async Task<byte[]> ConvetImgToByte(IFormFile formFile)
        {
            using (var imgbyte = new MemoryStream())
            {
                formFile.CopyTo(imgbyte);
                return imgbyte.ToArray();
            }
        }


        private byte[] CreateThumbnail(int width , int height, Stream image)
        {
            try
            {
                var origin = Image.FromStream(image);
                var reduced = origin.GetThumbnailImage(width, height, () => false, IntPtr.Zero);
                using (var create = new MemoryStream())
                {
                    reduced.Save(create, ImageFormat.Jpeg);
                    return create.ToArray();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpGet]
        [Route("/getproductimgbyid/{id:int}")]
        public async Task<IActionResult>GetProductimgById([FromRoute]int id)
        {
            var productimgDM = await _ProductRepository.GetProductImgById(id);
            if (productimgDM == null)
            {
                return NotFound();
            }
            byte[] format = productimgDM.productimg_img;

            return File(format,"image/jpeg");
        }

        [HttpGet]
        [Route("/GetImgByProductId/{id:int}/{index:int}")]
        public async Task<IActionResult> GetImgByProductId([FromRoute] int id, int index)
        {

            var productimgDM = await _ProductRepository.GetProductImgsById(id, index);
            if (productimgDM == null)
            {
                return NoContent();
            }
            byte[] format = productimgDM.productimg_img;

            return File(format, "image/jpeg");
        }

        [HttpGet]
        [Route("GetProductByUserId/{userid}")]
        public IActionResult GetProductByUserId([FromRoute] int userid) 
        {
            try
            {
                var ProductDM = _ProductRepository.GetProductByUserId(userid);
                if (ProductDM.Count == 0)
                {
                    return NotFound(new { Message = "No items found"});
                }
                return Ok(ProductDM);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Could not connect to database", error = ex.Message });
            }

        }

        [HttpGet]
        [Route("/GetImgThumbnailByProductId/{id:int}/{index:int}")]
        public async Task<IActionResult> GetImgThumbnailByProductId([FromRoute] int id, int index)
        {
            var productimgDM = await _ProductRepository.GetProductImgsThumbnailById(id, index);
            if (productimgDM == null)
            {
                return NoContent();
            }
            byte[] format = productimgDM.productimg_thumbnail;

            return File(format, "image/jpeg");
        }

        [HttpGet]
        [Route("/GetProductsByStatus/{status:int}")]
        public IActionResult GetProductsByStatus([FromRoute] int status)
        {
            var productDM = _ProductRepository.GetProductByStatus(status);
            if (productDM.Count == 0)
            {
                return NotFound();
            }
            return Ok(productDM);
        }

        [HttpPut]
        [Route("/ChangeProductStatus/{id:int}")]
        public IActionResult UpdateProductStatus([FromRoute] int id, [FromForm] UpdateProductStatusDTO updateProductStatusDTO) 
        {
            var validproduct = _ProductRepository.GetProductById(id);
            if (validproduct == null)
            {
                return NotFound(new { Message = "Product does not exist" });
            }
            var validstatus = _ProductRepository.ValidateStatus(updateProductStatusDTO.product_status);
            if (validstatus == null)
            {
                return NotFound(new { Message = "Status does not exist" });
            }
            var productDM = _ProductRepository.UpdateStatus(id, updateProductStatusDTO);
            
            return Ok(productDM);
        }

        [HttpGet]
        [Route("/SearchByProductName/{product_name}")]
        public IActionResult SearchProducts([FromRoute] string product_name)
        {
            var productDM = _ProductRepository.SearchProducts(product_name);
            if (productDM.Count == 0)
            {
                return NotFound(new { Message = "No Products found" });
            }
            return Ok(productDM);
        }
    }

}
