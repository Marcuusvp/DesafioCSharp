using OrdersApi.Models;
using OrdersApi.Models.Dtos;
using OrdersApi.Repository;
using Microsoft.EntityFrameworkCore;

namespace OrdersApi.Services
{
    public class OrderService : IOrderService
    {
        private readonly OrdersDbContext _context;

        public OrderService(OrdersDbContext context)
        {
            _context = context;
        }

        public async Task<Order> CreateOrderAsync(CreateOrderDto orderDto)
        {
            try
            {
                // Validate input data
                if (string.IsNullOrWhiteSpace(orderDto.User))
                {
                    throw new ArgumentException("User is required", nameof(orderDto.User));
                }

                if (orderDto.Items == null || orderDto.Items.Count == 0)
                {
                    throw new ArgumentException("At least one order item is required", nameof(orderDto.Items));
                }

                // Fetch items from the database
                var itemIds = new List<int>();
                foreach (var item in orderDto.Items)
                {
                    itemIds.Add(item.ItemId);
                }

                var items = await _context.Items
                    .Where(i => itemIds.Contains(i.Id))
                    .ToListAsync();

                if (items.Count != itemIds.Count)
                {
                    throw new ArgumentException("One or more items are invalid");
                }

                // Map DTO to domain models
                var orderItems = new List<OrderItem>();
                foreach (var itemDto in orderDto.Items)
                {
                    var item = items.FirstOrDefault(i => i.Id == itemDto.ItemId);
                    if (item == null)
                    {
                        throw new ArgumentException($"Item with ID {itemDto.ItemId} not found");
                    }

                    var orderItem = new OrderItem(item, itemDto.Quantity);
                    orderItems.Add(orderItem);
                }

                // Create the order
                var order = Order.CreateOrder(orderDto.User, orderItems, DateTime.UtcNow);

                // Add to the database
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                return order;
            }
            catch (DbUpdateException dbEx)
            {
                // Handle database update exceptions
                throw new Exception("An error occurred while saving the order.", dbEx);
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                throw new Exception("An unexpected error occurred.", ex);
            }            
        }
        public async Task CancelOrderAsync(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);

            if (order == null)
                throw new ArgumentException("Order not found");

            order.Cancel();
            await _context.SaveChangesAsync();
        }
    }
}
