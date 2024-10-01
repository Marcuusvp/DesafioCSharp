using OrdersApi.Models;

namespace OrdersApi.PaymentMethods.Interfaces
{
    public interface IPaymentStrategy
    {
        Task<bool> ProcessPaymentAsync(Order order);
    }

}
