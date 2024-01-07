using Grimsby_and_Clee_Sells.Data;
using Grimsby_and_Clee_Sells.Models.Domain;
using Grimsby_and_Clee_Sells.Models.DTOs;
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
        public IActionResult CreateProduct([FromBody] CreateProductDTO createProductDTO)
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

                _ProductRepository.CreateProduct(ProductDM);
                var CreateProductDTO = new ProductDTO
                {
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
                    await _ProductRepository.CreateProductImg(ProductDM);
                    var productDTO = new productimgDTO
                    {
                        productimg_id = ProductDM.productimg_id,
                        productimg_img = ProductDM.productimg_img,
                        productimg_productid = ProductDM.productimg_productid
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
                return NotFound();
            }
            byte[] format = productimgDM.productimg_img;

            return File(format, "image/jpeg");
        }

    }

}
