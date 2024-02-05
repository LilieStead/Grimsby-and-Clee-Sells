using Grimsby_and_Clee_Sells.Models.Domain;
using Grimsby_and_Clee_Sells.Models.DTOs;

namespace Grimsby_and_Clee_Sells.Repositories
{
    public interface IProductRepository
    {
        List<Product> GetAllProduct();

        Product GetProductById(int id);

        Product CreateProduct(Product product);

        Task<Productimg> GetProductImgById(int id);

        Task<Productimg> CreateProductImg(Productimg productimg);

        Task<Productimg> GetProductImgsById(int id, int index);

        List <Product> GetProductByUserId(int userId);

        Task<Productimg> GetProductImgsThumbnailById(int id, int index);

        List<Product> GetProductByStatus (int status);

        Product UpdateStatus (int id, UpdateProductStatusDTO updateProductStatusDTO);
        Product UpdateProduct(int productID, Product product);
        Status ValidateStatus (int id);

        List<Product> SearchProducts(string product_name);
    }
}
