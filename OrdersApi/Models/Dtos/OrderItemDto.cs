using System.ComponentModel.DataAnnotations;

namespace OrdersApi.Models.Dtos
{
    public class OrderItemDto
    {
        [Required]
        public int ItemId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
    }
}
