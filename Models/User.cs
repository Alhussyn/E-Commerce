namespace E_Commerce.Models
{
    public class User
    {
        public int Id { get; set; } // Primary Key

        public string Name { get; set; } // اسم المستخدم

        public string Email { get; set; } // الايميل

        public string Password { get; set; } // الباسورد

        public string Address { get; set; } // العنوان

        //  علاقة: User عنده Orders كتير
        public List<Order> Orders { get; set; }
    }
}
