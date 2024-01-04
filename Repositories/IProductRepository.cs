using Grimsby_and_Clee_Sells.Models.Domain;
using Grimsby_and_Clee_Sells.Models.DTOs;

namespace Grimsby_and_Clee_Sells.Repositories
{
    public interface IProductRepository
    {
        List<Product> GetAllProduct();
    }
}
