
using System.ComponentModel.DataAnnotations;

namespace Marketplace.Application.DTOs.ShoppingCart
{
    public class AddToCartDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; } = 1;
    }
}