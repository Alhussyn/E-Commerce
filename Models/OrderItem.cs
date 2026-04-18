namespace E_Commerce.Models
{
    public class OrderItem
    {
        public int Id { get; set; } // Primary Key

        public int OrderId { get; set; } // FK للأوردر

        public int ProductId { get; set; } // FK للمنتج

        public int Quantity { get; set; } // الكمية

        public decimal Price { get; set; } // سعر المنتج وقت الشراء

        //  Navigation
        public Order Order { get; set; }

        public Product Product { get; set; }
    }
}
