using OrdersApi.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace OrdersApi.Models.Dtos
{
    public class ProcessPaymentDto
    {
        [Required]
        public PaymentMethod PaymentMethod { get; set; }
    }

}
