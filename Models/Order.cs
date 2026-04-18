namespace E_Commerce.Models
{
    public class Order
    {
        public int Id { get; set; } // Primary Key

        public int UserId { get; set; } // Foreign Key

        public decimal TotalPrice { get; set; } // السعر الكلي للأوردر

        //  Navigation
        public User User { get; set; }

        //  المنتجات داخل الأوردر
        public List<OrderItem> OrderItems { get; set; }
    }
}
