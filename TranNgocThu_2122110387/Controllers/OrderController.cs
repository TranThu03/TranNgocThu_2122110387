using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TranNgocThu_2122110387.Data;
using TranNgocThu_2122110387.Model;

namespace TranNgocThu_2122110387.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrderController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Order
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            var orders = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .ToListAsync();

            return Ok(orders);
        }

        // GET: api/Order/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        // POST: api/Order
        [HttpPost]
        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder([FromBody] Order order)
        {
            if (order == null || order.OrderDetails == null || !order.OrderDetails.Any())
            {
                return BadRequest("Order and OrderDetails are required.");
            }

            // Validate if User exists
            var user = await _context.Users.FindAsync(order.UserId);
            if (user == null)
            {
                return BadRequest("Invalid User. User with the given ID does not exist.");
            }

            // Calculate total amount
            decimal totalAmount = 0;
            foreach (var orderDetail in order.OrderDetails)
            {
                var product = await _context.Products.FindAsync(orderDetail.ProductId);
                if (product == null)
                {
                    return BadRequest($"Product with ID {orderDetail.ProductId} not found.");
                }

                // Convert string Price to decimal
                if (decimal.TryParse(product.Price, out decimal price))
                {
                    totalAmount += price * orderDetail.Quantity;
                }
                else
                {
                    return BadRequest($"Invalid price format for product with ID {orderDetail.ProductId}.");
                }
            }

            order.TotalAmount = totalAmount;
            order.OrderDate = DateTime.UtcNow;

            // Save order
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrder", new { id = order.OrderId }, order);
        }


        // DELETE: api/Order/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent(); // Indicate the deletion was successful
        }
    }
}
