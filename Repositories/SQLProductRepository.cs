using Grimsby_and_Clee_Sells.Controllers;
using Grimsby_and_Clee_Sells.Data;
using Grimsby_and_Clee_Sells.Models.Domain;
using Grimsby_and_Clee_Sells.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Grimsby_and_Clee_Sells.Repositories
{

    public class SQLProductRepository : IProductRepository
    {
        private readonly GacsDbContext _context;

        public SQLProductRepository(GacsDbContext context)
        {
            this._context = context;
        }

        public List<Product> GetAllProduct()
        {
            return _context.Tbl_Product.Include(p => p.Status).Include(p => p.Category).Include(p => p.User).ToList();
        }

        public Product GetProductById(int id)
        {
            return _context.Tbl_Product.Include(p => p.Status).Include(p => p.Category).Include(p => p.User).FirstOrDefault(p => p.product_id == id);
        }

        public Product CreateProduct(Product product) {
            _context.Tbl_Product.Add(product);
            _context.SaveChanges();
            return product;
        }

        public async Task<Productimg> GetProductImgById(int id)
        {
            return await _context.Tbl_Productimg.Include(p => p.Product).Select(p => new Productimg
            {
                productimg_id = p.productimg_id,
                productimg_img = p.productimg_img,
                productimg_productid = p.productimg_productid
            }).FirstOrDefaultAsync(p => p.productimg_id == id);
        }

        public async Task<Productimg> GetProductImgsById(int id, int index)
        {
            return await _context.Tbl_Productimg.Include(p => p.Product)
                .Where(p => p.productimg_productid == id)
                .OrderBy(p => p.productimg_id)
                .Skip(index)
                .Select(p => new Productimg
            {
                productimg_id = p.productimg_id,
                productimg_img = p.productimg_img,
                productimg_productid = p.productimg_productid
            }).FirstOrDefaultAsync();
        }

        public async Task<Productimg> CreateProductImg(Productimg productimg)
        {
            _context.Tbl_Productimg.AddAsync(productimg);
            await _context.SaveChangesAsync();
            return productimg;
        }

        public List<Product> GetProductByUserId(int userid)
        {
            return _context.Tbl_Product.Include(p => p.Status).Include(p => p.Category).Include(p => p.User).Where(p => p.product_userid == userid).ToList();
        }

        public async Task<Productimg> GetProductImgsThumbnailById(int id, int index)
        {
            return await _context.Tbl_Productimg.Include(p => p.Product)
                .Where(p => p.productimg_productid == id)
                .OrderBy(p => p.productimg_id)
                .Skip(index)
                .Select(p => new Productimg
                {
                    productimg_id = p.productimg_id,
                    productimg_thumbnail = p.productimg_thumbnail,
                    productimg_productid = p.productimg_productid
                }).FirstOrDefaultAsync();
        }

        public List<Product> GetProductByStatus(int status)
        {
            return _context.Tbl_Product.Include(p => p.Status).Include(p => p.Category).Include(p => p.User).Where(p => p.product_status == status).ToList();
        }


        public Product UpdateStatus(int id, UpdateProductStatusDTO updateProductStatusDTO)
        {
            var product = _context.Tbl_Product.FirstOrDefault(p => p.product_id == id);
            product.product_status = updateProductStatusDTO.product_status;
            _context.SaveChanges();
            return product;
        }

        public Status ValidateStatus(int id)
        {
            return _context.Tbl_Status.FirstOrDefault(p => p.status_id == id);
        }
    }
}
