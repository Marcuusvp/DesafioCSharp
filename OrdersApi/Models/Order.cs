using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using OrdersApi.Models.Enums;

namespace OrdersApi.Models
{
    public enum OrderStatus
    {
        AguardandoProcessamento,
        ProcessandoPagamento,
        PagamentoConcluido,
        SeparandoPedido,
        AguardandoEstoque,
        Concluido,
        Cancelado
    }

    public sealed class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; }

        [Required]
        public string User { get; }

        [Required]
        public List<OrderItem> Items { get; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; private set; }

        [Required]
        public OrderStatus Status { get; private set; }

        public DateTime CreatedAt { get; }

        private Order()
        {
            Items = new List<OrderItem>();
        }

        private Order(string user, List<OrderItem> items, DateTime createdAt)
        {
            User = user;
            Items = items;
            CreatedAt = createdAt;
            Status = OrderStatus.AguardandoProcessamento;
            CalculateTotal();
        }

        public static Order CreateOrder(string user, List<OrderItem> items, DateTime createdAt)
        {
            return new Order(user, items, createdAt);
        }

        private void CalculateTotal()
        {
            decimal total = 0m;

            foreach (var orderItem in Items)
            {
                decimal itemTotal = orderItem.Item.Price * orderItem.Quantity;

                // Apply discount by quantity
                decimal quantityDiscount = ApplyQuantityDiscount(orderItem);
                itemTotal -= quantityDiscount;

                total += itemTotal;
            }

            // Apply seasonal discount
            decimal seasonalDiscount = ApplySeasonalDiscount(total, CreatedAt);
            total -= seasonalDiscount;

            Total = total;
        }
        private decimal ApplyQuantityDiscount(OrderItem orderItem)
        {
            // Example: For quantities of 10 or more, discount $5 per item
            if (orderItem.Quantity >= 10)
            {
                return 5m * orderItem.Quantity;
            }
            return 0m;
        }
        public decimal ApplySeasonalDiscount(decimal subtotal, DateTime orderDate)
        {
            // Example: In December, apply a 10% discount
            if (orderDate.Month == 12)
            {
                return subtotal * 0.10m;
            }
            return 0m;
        }
        public void CancelOrder()
        {
            if (Status == OrderStatus.AguardandoProcessamento)
            {
                Status = OrderStatus.Cancelado;
            }
            else
            {
                throw new InvalidOperationException("Cannot cancel an order that is not awaiting processing.");
            }
        }
        public void UpdateStatus(OrderStatus newStatus)
        {
            Status = newStatus;
        }

        public void ApplyDiscount(decimal discountPercentage)
        {
            Total -= Total * discountPercentage;
        }

        public void ApplyPaymentDiscount(PaymentMethod paymentMethod)
        {
            if (paymentMethod == PaymentMethod.Pix)
            {
                ApplyDiscount(0.05m);
            }
        }
        public void Cancel()
        {
            if (Status == OrderStatus.PagamentoConcluido)
            {
                // Perform refund operation
                // If refund successful:
                UpdateStatus(OrderStatus.Cancelado);
            }
            else if (Status == OrderStatus.AguardandoProcessamento || Status == OrderStatus.ProcessandoPagamento)
            {
                UpdateStatus(OrderStatus.Cancelado);
            }
            else
            {
                throw new InvalidOperationException("Order cannot be canceled at this stage");
            }
        }
        public void StartSeparation()
        {
            if (Status != OrderStatus.PagamentoConcluido)
                throw new InvalidOperationException("Order is not ready for separation.");

            UpdateStatus(OrderStatus.SeparandoPedido);
        }

        public void CompleteOrder()
        {
            if (Status != OrderStatus.SeparandoPedido)
                throw new InvalidOperationException("Order is not in separation process.");

            UpdateStatus(OrderStatus.Concluido);
        }

        public void SetAwaitingStock()
        {
            if (Status != OrderStatus.SeparandoPedido)
                throw new InvalidOperationException("Order is not in separation process.");

            UpdateStatus(OrderStatus.AguardandoEstoque);
        }

    }
}
