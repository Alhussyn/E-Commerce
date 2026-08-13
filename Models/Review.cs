using System.ComponentModel.DataAnnotations;

namespace E_Commerce.Models
{
    public class Review
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int ProductId { get; set; }

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }

        [StringLength(1000)]
        public string Comment { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User? User { get; set; }

        public Product? Product { get; set; }
    }
}
