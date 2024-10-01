using OrdersApi.Models;
using OrdersApi.Models.Dtos;
using System.Threading.Tasks;

namespace OrdersApi.Services
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(CreateOrderDto orderDto);
        Task CancelOrderAsync(int orderId);
    }
}
