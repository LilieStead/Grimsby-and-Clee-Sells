using Grimsby_and_Clee_Sells.Data;
using Grimsby_and_Clee_Sells.Models.Domain;
using Microsoft.EntityFrameworkCore;

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
        {//make order 
            _context.Tbl_Order.Add(order);
            //add to database 
            _context.SaveChanges();
            //save 
            return order;
        }
        public OrderProduct OrderProduct(OrderProduct product)
        {
            //add an order to database
            _context.Tbl_OrderProduct.Add(product);
            //save the chnages 
            _context.SaveChanges();
            return product;
        }

        public Product ProductIsSold(int productID, int quantity)
        {//add 1 to the products sold eneity 
            var product = _context.Tbl_Product.FirstOrDefault(p => p.product_id == productID);
            if (product == null)
            {
                //if its unable to return nothing
                return null;
            }

            product.product_sold += quantity;
            //save chnage to database
            _context.SaveChanges();
            return product;
        }

        public Order GetOrderByID(int id)
        {
            //get the otder based on the id and incude the status
            return _context.Tbl_Order.Include(p => p.Orderstatus).FirstOrDefault(p => p.order_id == id);
        }

        public Order DeleteOrder(int id)
        {
            //detlet the first order based on the order id 
            var order = _context.Tbl_Order.FirstOrDefault(p => p.order_id == id);
            _context.Remove(order);
            //save changes 
            _context.SaveChanges();
            return order;
        }

        public OrderProduct RemoveOrderedProducts(int orderID, int productID)
        {
            //reomve products from order
            var product = _context.Tbl_OrderProduct.Where(p => p.orderproducts_productid == productID).FirstOrDefault(p => p.orderproducts_orderid == orderID);
            _context.Tbl_OrderProduct.Remove(product);
            //save changes
            _context.SaveChanges();
            return product;
        }

        public List<Order> GetOrderByUserId(int userId)
        {//get order by user id
            return _context.Tbl_Order.Where(p => p.order_userid == userId).Include(p => p.Orderstatus).ToList();
        }
    }
}
