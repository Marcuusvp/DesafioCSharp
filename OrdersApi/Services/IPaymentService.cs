using OrdersApi.Models.Enums;

namespace OrdersApi.Services
{
    public interface IPaymentService
    {
        Task ProcessPaymentAsync(int orderId, PaymentMethod paymentMethod);
    }
}
