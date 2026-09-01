
using System.ComponentModel.DataAnnotations;

namespace Marketplace.Application.DTOs.Orders
{
    public class CreateOrderDto
    {
        [Required(ErrorMessage = "Shipping address is required")]
        [StringLength(200)]
        public string ShippingAddress { get; set; } = string.Empty;

        [StringLength(50)]
        public string? ShippingCity { get; set; }

        [StringLength(50)]
        public string? ShippingCountry { get; set; }

        [StringLength(20)]
        public string? ShippingPostalCode { get; set; }

        [Required(ErrorMessage = "Payment method is required")]
        [StringLength(50)]
        public string PaymentMethod { get; set; } = string.Empty;

        public string? Notes { get; set; }
        public List<CreateOrderItemDto> Items { get; set; } = new();
    }
}