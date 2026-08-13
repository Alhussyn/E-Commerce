using E_Commerce.Data;
using E_Commerce.Models;
using E_Commerce.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

namespace E_Commerce.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly PasswordHasher<User> _passwordHasher = new PasswordHasher<User>();

        public AuthController(AppDbContext context, IEmailService emailService, IConfiguration configuration)
        {
            _context = context;
            _emailService = emailService;
            _configuration = configuration;
        }

        // ================= SIGN UP =================

        [HttpGet]
        public IActionResult SignUp()
        {
            ViewBag.mode = "signup";
            return View("auth");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SignUp(User user)
        {
            if (user.Name == null || user.Email == null || user.Password == null || user.Address == null)
            {
                ViewBag.error = "Please fill all inputs";
                ViewBag.mode = "signup";
                return View("auth");
            }

            var entryuser = _context.Users.FirstOrDefault(u => u.Email == user.Email);
            if (entryuser != null)
            {
                ViewBag.error = "This email already exists, please login";
                ViewBag.mode = "signin";
                return View("auth");
            }

            var adminEmail = _configuration["AdminSettings:DefaultAdminEmail"];
            var adminPassword = _configuration["AdminSettings:DefaultAdminPassword"];

            user.Role = (!string.IsNullOrEmpty(adminEmail) && !string.IsNullOrEmpty(adminPassword)
                && user.Email == adminEmail && user.Password == adminPassword)
                ? "Manager" : "User";

            user.PasswordHash = _passwordHasher.HashPassword(user, user.Password);
            user.Password = string.Empty;

            _context.Users.Add(user);
            _context.SaveChanges();

            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserName", user.Name);
            HttpContext.Session.SetString("UserRole", user.Role);

            return user.Role == "Manager"
                ? RedirectToAction("Index", "Manage")
                : RedirectToAction("Index", "Product");
        }

        // ================= SIGN IN =================

        [HttpGet]
        public IActionResult SignIn()
        {
            ViewBag.mode = "signin";
            return View("auth");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SignIn(string email, string password)
        {
            if (email == null || password == null)
            {
                ViewBag.error = "Please fill all data";
                ViewBag.mode = "signin";
                return View("auth");
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user != null)
            {
                bool passwordValid = false;

                if (!string.IsNullOrEmpty(user.PasswordHash))
                {
                    var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
                    passwordValid = result == PasswordVerificationResult.Success
                                 || result == PasswordVerificationResult.SuccessRehashNeeded;
                }
                else if (!string.IsNullOrEmpty(user.Password))
                {
                    passwordValid = user.Password == password;
                    if (passwordValid)
                    {
                        user.PasswordHash = _passwordHasher.HashPassword(user, password);
                        user.Password = string.Empty;
                        _context.SaveChanges();
                    }
                }

                if (passwordValid)
                {
                    HttpContext.Session.SetInt32("UserId", user.Id);
                    HttpContext.Session.SetString("UserName", user.Name);
                    HttpContext.Session.SetString("UserRole", user.Role);

                    return user.Role == "Manager"
                        ? RedirectToAction("Index", "Manage")
                        : RedirectToAction("Index", "Product");
                }
            }

            ViewBag.error = "Invalid email or password";
            ViewBag.mode = "signin";
            return View("auth");
        }

        // ================= LOGOUT =================

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("SignIn");
        }

        // ================= FORGOT PASSWORD =================

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                ViewBag.Error = "Please enter your email address.";
                return View();
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                ViewBag.Success = "If that email is registered, an OTP has been sent.";
                return View();
            }

            var otp = new Random().Next(100000, 999999).ToString();
            user.OtpCode   = otp;
            user.OtpExpiry = DateTime.UtcNow.AddMinutes(10);
            _context.SaveChanges();

            var body = $@"
                <div style='font-family:Arial,sans-serif;max-width:480px;margin:auto;
                            border:1px solid #e0e0e0;border-radius:12px;padding:32px;'>
                    <h2 style='color:#1a1a2e;margin-bottom:8px;'>Password Reset OTP</h2>
                    <p style='color:#555;'>Use the code below to reset your password.
                       It expires in <strong>10 minutes</strong>.</p>
                    <div style='background:#f4f6ff;border-radius:8px;padding:24px;
                                text-align:center;margin:24px 0;'>
                        <span style='font-size:36px;font-weight:bold;letter-spacing:8px;
                                     color:#4361ee;'>{otp}</span>
                    </div>
                    <p style='color:#888;font-size:13px;'>
                        If you did not request this, please ignore this email.
                    </p>
                </div>";

            try
            {
                await _emailService.SendEmailAsync(email, "Your Password Reset OTP", body);
            }
            catch
            {
                ViewBag.Error = "Failed to send email. Please check SMTP settings.";
                return View();
            }

            HttpContext.Session.SetString("OtpEmail", email);

            return RedirectToAction("VerifyOtp");
        }

        // ================= VERIFY OTP =================

        [HttpGet]
        public IActionResult VerifyOtp()
        {
            if (HttpContext.Session.GetString("OtpEmail") == null)
                return RedirectToAction("ForgotPassword");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult VerifyOtp(string otp)
        {
            var email = HttpContext.Session.GetString("OtpEmail");
            if (email == null)
                return RedirectToAction("ForgotPassword");

            if (string.IsNullOrWhiteSpace(otp))
            {
                ViewBag.Error = "Please enter the OTP.";
                return View();
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null || user.OtpCode != otp || user.OtpExpiry < DateTime.UtcNow)
            {
                ViewBag.Error = "Invalid or expired OTP. Please try again.";
                return View();
            }

            HttpContext.Session.SetString("OtpVerified", "true");

            return RedirectToAction("ResetPassword");
        }

        // ================= RESET PASSWORD =================

        [HttpGet]
        public IActionResult ResetPassword()
        {
            if (HttpContext.Session.GetString("OtpEmail") == null ||
                HttpContext.Session.GetString("OtpVerified") != "true")
                return RedirectToAction("ForgotPassword");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ResetPassword(string newPassword, string confirmPassword)
        {
            var email = HttpContext.Session.GetString("OtpEmail");

            if (email == null || HttpContext.Session.GetString("OtpVerified") != "true")
                return RedirectToAction("ForgotPassword");

            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
            {
                ViewBag.Error = "Password must be at least 6 characters.";
                return View();
            }

            if (newPassword != confirmPassword)
            {
                ViewBag.Error = "Passwords do not match.";
                return View();
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                ViewBag.Error = "User not found.";
                return View();
            }

            user.PasswordHash = _passwordHasher.HashPassword(user, newPassword);
            user.Password = string.Empty;
            user.OtpCode   = null;
            user.OtpExpiry = null;
            _context.SaveChanges();

            HttpContext.Session.Remove("OtpEmail");
            HttpContext.Session.Remove("OtpVerified");

            ViewBag.mode    = "signin";
            ViewBag.Success = "Password reset successfully! Please sign in.";
            return View("auth");
        }
    }
}
