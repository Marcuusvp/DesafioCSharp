using OrdersApi.Models.Enums;
using OrdersApi.Models;
using OrdersApi.PaymentMethods.Interfaces;
using OrdersApi.PaymentMethods;
using OrdersApi.Repository;
using OrdersApi.Services;
using Microsoft.EntityFrameworkCore;

public class PaymentService : IPaymentService
{
    private readonly OrdersDbContext _context;

    public PaymentService(OrdersDbContext context)
    {
        _context = context;
    }

    public async Task ProcessPaymentAsync(int orderId, PaymentMethod paymentMethod)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .ThenInclude(oi => oi.Item)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null)
            throw new ArgumentException("Order not found");

        if (order.Status != OrderStatus.AguardandoProcessamento)
            throw new InvalidOperationException("Order is not awaiting processing");

        order.UpdateStatus(OrderStatus.ProcessandoPagamento);
        await _context.SaveChangesAsync();

        IPaymentStrategy paymentStrategy = paymentMethod switch
        {
            PaymentMethod.Pix => new PixPaymentStrategy(),
            PaymentMethod.CreditCard => new CreditCardPaymentStrategy(),
            _ => throw new NotSupportedException("Payment method not supported"),
        };

        bool paymentSuccess = false;
        int attempts = 0;
        const int maxAttempts = 3;

        while (!paymentSuccess && attempts < maxAttempts)
        {
            try
            {
                paymentSuccess = await paymentStrategy.ProcessPaymentAsync(order);
            }
            catch (Exception ex) when (IsCommunicationException(ex))
            {
                attempts++;
                if (attempts >= maxAttempts)
                {
                    order.UpdateStatus(OrderStatus.Cancelado);
                    await _context.SaveChangesAsync();
                    throw new Exception("Payment processing failed after multiple attempts");
                }
            }
        }

        if (paymentSuccess)
        {
            order.ApplyPaymentDiscount(paymentMethod);
            order.UpdateStatus(OrderStatus.PagamentoConcluido);
            await _context.SaveChangesAsync();
        }
    }

    private bool IsCommunicationException(Exception ex)
    {
        // Implement logic to determine if the exception is a communication failure
        return ex is TimeoutException || ex is HttpRequestException;
    }
}
