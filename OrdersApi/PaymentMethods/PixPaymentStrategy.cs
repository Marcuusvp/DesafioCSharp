using OrdersApi.Models;
using OrdersApi.PaymentMethods.Interfaces;

namespace OrdersApi.PaymentMethods
{
    public class PixPaymentStrategy : IPaymentStrategy
    {
        public async Task<bool> ProcessPaymentAsync(Order order)
        {
            await Task.Delay(500);

            return true;
        }
    }

}
