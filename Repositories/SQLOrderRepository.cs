using Grimsby_and_Clee_Sells.Data;
using Grimsby_and_Clee_Sells.Models.Domain;

namespace Grimsby_and_Clee_Sells.Repositories
{
    public class SQLOrderRepository : IOrderRepository
    {

        private readonly GacsDbContext _context;

        public SQLOrderRepository(GacsDbContext context)
        {
            this._context = context;
        }


        public Order CreateOrder(Order order)
        {
            _context.Tbl_Order.Add(order);
            _context.SaveChanges();
            return order;
        }
        public OrderProduct OrderProduct(OrderProduct product)
        {
            _context.Tbl_OrderProduct.Add(product);
            _context.SaveChanges();
            return product;
        }

        public Product ProductIsSold(int productID, int quantity)
        {
            var product = _context.Tbl_Product.FirstOrDefault(p => p.product_id == productID);
            if (product == null)
            {
                return null;
            }

            product.product_sold += quantity;
            _context.SaveChanges();
            return product;
        }
    }
}
