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
            //gets a list of all products
        {
            return _context.Tbl_Product.Include(p => p.Status).Include(p => p.Category).Include(p => p.User).ToList();
        }

        public Product GetProductById(int id)
        {//gets a product based on its id
            return _context.Tbl_Product.Include(p => p.Status).Include(p => p.Category).Include(p => p.User).FirstOrDefault(p => p.product_id == id);
        }

        public Product CreateProduct(Product product)
        {//creates a product
            _context.Tbl_Product.Add(product);
            //addes to database
            _context.SaveChanges();
            //saves changes 
            return product;
        }

        public Product UpdateProduct(int productID, Product product)
        {
            try
            {//vaildate product exists
                var existingProduct = _context.Tbl_Product.FirstOrDefault(p => p.product_id == productID);
                if (existingProduct == null)
                {
                    return null;
                }
                //fields to update
                existingProduct.product_price = product.product_price;
                existingProduct.product_description = product.product_description;
                existingProduct.product_status = 1;
                existingProduct.product_name = product.product_name;
                existingProduct.product_category = product.product_category;
                _context.SaveChanges();
                return existingProduct;
            }
            catch (Exception ex)
            {//could not find product
                return null;
            }
            
        }

        public async Task<Productimg> GetProductImgById(int id)
        {
            //get the image based on the product id
            return await _context.Tbl_Productimg.Include(p => p.Product).Select(p => new Productimg
            {
                productimg_id = p.productimg_id,
                productimg_img = p.productimg_img,
                productimg_productid = p.productimg_productid
            }).FirstOrDefaultAsync(p => p.productimg_id == id);
        }

        public async Task<Productimg> GetProductImgsById(int id, int index)
        {//get the image based on the product id
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
        {//adds a products image to the database
            _context.Tbl_Productimg.AddAsync(productimg);
            await _context.SaveChangesAsync();
            return productimg;
        }

        public List<Product> GetProductByUserId(int userid)
        {//gets a list of products based on a users id
            return _context.Tbl_Product.Include(p => p.Status).Include(p => p.Category).Include(p => p.User).Where(p => p.product_userid == userid).ToList();
        }

        public async Task<Productimg> GetProductImgsThumbnailById(int id, int index)
        {//get a thumbnail based on the product id
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

        public async Task<Productimg> UpdateImage(int productID, Productimg productimages, int index)
        {//update image (didnt get to doing this in the UI
            var productImg = await _context.Tbl_Productimg.OrderBy(p => p.productimg_id).Skip(index).FirstOrDefaultAsync(p => p.productimg_productid == productID);
            productImg.productimg_img = productimages.productimg_img;
            productImg.productimg_thumbnail = productimages.productimg_thumbnail;
            await _context.SaveChangesAsync();
            return productImg;
        }

        public List<Product> GetProductByStatus(int status)
        {//get a product based on a status 
            return _context.Tbl_Product.Include(p => p.Status).Include(p => p.Category).Include(p => p.User).Where(p => p.product_status == status).ToList();
        }


        public Product UpdateStatus(int id, UpdateProductStatusDTO updateProductStatusDTO)
        {//update a products status based on the product id
            var product = _context.Tbl_Product.FirstOrDefault(p => p.product_id == id);
            product.product_status = updateProductStatusDTO.product_status;
            _context.SaveChanges();
            return product;
        }

        public Status ValidateStatus(int id)
        {//makes sure the status is vaild
            return _context.Tbl_Status.FirstOrDefault(p => p.status_id == id);
        }

        public List<Product> SearchProducts(string product_name)
        {
            return _context.Tbl_Product.Include(p => p.User).Include(p => p.Category)
                .Where(p => p.product_status == 2 && p.product_name.Contains(product_name))
                .ToList();
        }

        public List<Product> TopProducts()
        {// returns the 5 top products 
            return _context.Tbl_Product.OrderByDescending(p => p.product_sold).Include(p => p.User).Include(p => p.Category).Where(p => p.product_status == 2).Take(5).ToList();
        }

    }
}
