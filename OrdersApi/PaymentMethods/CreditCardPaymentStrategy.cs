using OrdersApi.Models;
using OrdersApi.PaymentMethods.Interfaces;

namespace OrdersApi.PaymentMethods
{
    public class CreditCardPaymentStrategy : IPaymentStrategy
    {
        public async Task<bool> ProcessPaymentAsync(Order order)
        {
            // Simulate payment processing
            await Task.Delay(500);

            // Assume payment is successful
            return true;
        }
    }
}
