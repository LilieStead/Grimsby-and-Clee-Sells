using Grimsby_and_Clee_Sells.Models.Domain;

namespace Grimsby_and_Clee_Sells.Repositories
{
    public interface IOrderRepository
    {
        Order CreateOrder(Order order);
        OrderProduct OrderProduct(OrderProduct product);
        Product ProductIsSold(int productID, int quantity);
        Order GetOrderByID(int orderID);
        Order DeleteOrder(int id);
        OrderProduct RemoveOrderedProducts(int orderID, int productID);
    }
}
