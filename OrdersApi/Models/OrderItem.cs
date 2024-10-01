using Microsoft.EntityFrameworkCore;

namespace OrdersApi.Models
{
    [Owned]
    public sealed class OrderItem
    {
        public Item Item { get; private set; }
        public int Quantity { get; private set; }

        private OrderItem() { }

        public OrderItem(Item item, int quantity)
        {
            Item = item;
            Quantity = quantity;
        }
    }
}
