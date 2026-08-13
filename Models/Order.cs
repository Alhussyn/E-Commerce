namespace E_Commerce.Models
{
    public class Order
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public decimal TotalPrice { get; set; }

        public DateTime dateTime { get; set; } = DateTime.UtcNow;

        public string Status { get; set; } = "Pending";

        public string? FullName { get; set; }

        public string? Phone { get; set; }

        public string? Address { get; set; }

        public string? City { get; set; }

        public string? Notes { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public User? User { get; set; }

        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
