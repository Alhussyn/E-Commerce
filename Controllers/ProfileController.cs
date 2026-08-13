using E_Commerce.Data;
using E_Commerce.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Controllers
{
    public class ProfileController : Controller
    {
        private readonly AppDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher = new PasswordHasher<User>();

        public ProfileController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("SignIn", "Auth");

            var user = _context.Users.Find(userId.Value);
            if (user == null)
                return RedirectToAction("SignIn", "Auth");

            var orderCount = _context.Orders.Count(o => o.UserId == userId.Value);
            var wishlistCount = _context.Wishlists.Count(w => w.UserId == userId.Value);

            ViewBag.OrderCount = orderCount;
            ViewBag.WishlistCount = wishlistCount;

            user.Password = string.Empty;
            user.PasswordHash = string.Empty;
            user.OtpCode = null;
            user.OtpExpiry = null;

            return View(user);
        }

        [HttpGet]
        public IActionResult Edit()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("SignIn", "Auth");

            var user = _context.Users.Find(userId.Value);
            if (user == null)
                return RedirectToAction("SignIn", "Auth");

            user.Password = string.Empty;
            user.PasswordHash = string.Empty;
            user.OtpCode = null;
            user.OtpExpiry = null;

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string name, string address)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("SignIn", "Auth");

            var user = _context.Users.Find(userId.Value);
            if (user == null)
                return RedirectToAction("SignIn", "Auth");

            if (string.IsNullOrWhiteSpace(name) || name.Length < 3)
            {
                ViewBag.Error = "Name must be at least 3 characters.";
                return View(user);
            }

            if (string.IsNullOrWhiteSpace(address))
            {
                ViewBag.Error = "Address is required.";
                return View(user);
            }

            user.Name = name.Trim();
            user.Address = address.Trim();
            _context.SaveChanges();

            HttpContext.Session.SetString("UserName", user.Name);
            TempData["Success"] = "Profile updated successfully.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("SignIn", "Auth");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("SignIn", "Auth");

            var user = _context.Users.Find(userId.Value);
            if (user == null)
                return RedirectToAction("SignIn", "Auth");

            if (string.IsNullOrWhiteSpace(currentPassword))
            {
                ViewBag.Error = "Please enter your current password.";
                return View();
            }

            bool currentValid = false;
            if (!string.IsNullOrEmpty(user.PasswordHash))
            {
                var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, currentPassword);
                currentValid = result == PasswordVerificationResult.Success || result == PasswordVerificationResult.SuccessRehashNeeded;
            }
            else if (!string.IsNullOrEmpty(user.Password))
            {
                currentValid = user.Password == currentPassword;
            }

            if (!currentValid)
            {
                ViewBag.Error = "Current password is incorrect.";
                return View();
            }

            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
            {
                ViewBag.Error = "New password must be at least 6 characters.";
                return View();
            }

            if (newPassword != confirmPassword)
            {
                ViewBag.Error = "Passwords do not match.";
                return View();
            }

            user.PasswordHash = _passwordHasher.HashPassword(user, newPassword);
            user.Password = string.Empty;
            _context.SaveChanges();

            TempData["Success"] = "Password changed successfully.";
            return RedirectToAction("Index");
        }
    }
}
