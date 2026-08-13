namespace E_Commerce.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public string ProductName { get; set; } = string.Empty;

        // Navigation
        public Order? Order { get; set; }

        public Product? Product { get; set; }
    }
}
