namespace E_Commerce.Models
{
    public class Product
    {
        public int Id { get; set; } // Primary Key

        public string Name { get; set; } // اسم المنتج

        public string Description { get; set; } // وصف المنتج

        public decimal Price { get; set; } // السعر الحالي

        public string ImageUrl { get; set; } // صورة المنتج

        public int Quantity { get; set; } // الكمية في المخزون

        // 🔗 المنتج ممكن يظهر في OrderItems كتير
        public List<OrderItem> OrderItems { get; set; }
    }
}