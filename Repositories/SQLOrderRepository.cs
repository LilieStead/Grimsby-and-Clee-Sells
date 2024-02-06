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

        public Order GetOrderByID(int id)
        {
            return _context.Tbl_Order.FirstOrDefault(p => p.order_id == id);
        }

        public Order DeleteOrder(int id)
        {
            var order = _context.Tbl_Order.FirstOrDefault(p => p.order_id == id);
            _context.Remove(order);
            _context.SaveChanges();
            return order;
        }

        public OrderProduct RemoveOrderedProducts(int orderID, int productID)
        {
            var product = _context.Tbl_OrderProduct.Where(p => p.orderproducts_productid == productID).FirstOrDefault(p => p.orderproducts_orderid == orderID);
            _context.Tbl_OrderProduct.Remove(product);
            _context.SaveChanges();
            return product;
        }
    }
}
