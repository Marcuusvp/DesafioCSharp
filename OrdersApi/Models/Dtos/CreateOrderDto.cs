using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OrdersApi.Models.Dtos
{
    public class CreateOrderDto
    {
        [Required]
        public string User { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "At least one order item is required")]
        public List<OrderItemDto> Items { get; set; }
    }
}
