
using System.ComponentModel.DataAnnotations;

namespace Marketplace.Application.DTOs.Orders
{
    public class CreateOrderItemDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}