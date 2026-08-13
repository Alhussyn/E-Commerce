using System.ComponentModel.DataAnnotations;

namespace E_Commerce.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, MinimumLength = 3,
            ErrorMessage = "Name must be between 3 and 100 characters")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; } = string.Empty;

        public string? PasswordHash { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [StringLength(200, ErrorMessage = "Address can't exceed 200 characters")]
        public required string Address { get; set; }

        public string Role { get; set; } = "User";

        // ===== OTP Fields =====
        public string? OtpCode { get; set; }
        public DateTime? OtpExpiry { get; set; }

        public List<Order> Orders { get; set; } = new List<Order>();
        public Cart? Cart { get; set; }
    }
}
