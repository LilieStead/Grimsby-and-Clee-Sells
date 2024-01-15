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
                };

                return CreatedAtAction("GetProductById", new { id = CreateProductDTO.product_id }, CreateProductDTO);

            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("/getproductbyid/{id:int}")]
        public IActionResult GetProductById([FromRoute] int id)
        {
            var ProductDM = _ProductRepository.GetProductById(id);

            if (ProductDM == null)
            {
                return NotFound();
            }

            return Ok(ProductDM);
        }


        [HttpPost]
        [Route("/createproductimg")]
        public async Task<IActionResult> CreateProductImg([FromForm]CreateProductimgDTO createProductimgDTO)
        {
            List<productimgDTO> productimgDTOs = new List<productimgDTO>();
            foreach(var img in createProductimgDTO.productimg_img)
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
            var ProductDM = _ProductRepository.GetProductByUserId(userid);
            if (ProductDM.Count == 0)
            {
                return NotFound();
            }
            return Ok(ProductDM);
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
    }

}
