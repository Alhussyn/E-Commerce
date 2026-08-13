using System.ComponentModel.DataAnnotations;

namespace E_Commerce.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 100 characters")]
        public required string Name { get; set; }

        [StringLength(500, ErrorMessage = "Description can't exceed 500 characters")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 1000000000, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Range(0, 100, ErrorMessage = "Discount must be between 0 and 100")]
        public decimal DiscountPercent { get; set; }

        [StringLength(64)]
        public string? SKU { get; set; }

        [StringLength(100)]
        public string? Brand { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required(ErrorMessage = "Image URL is required")]
        [StringLength(500, ErrorMessage = "Image path cannot exceed 500 characters")]
        public required string ImageUrl { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(0, 100000, ErrorMessage = "Quantity can't be negative")]
        public int Quantity { get; set; }

        public int? CategoryId { get; set; }

        public Category? Category { get; set; }

        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public List<ProductImage> Images { get; set; } = new List<ProductImage>();

        public decimal FinalPrice => Price * (1 - (DiscountPercent / 100));
    }
}
