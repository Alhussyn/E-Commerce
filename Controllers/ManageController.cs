using E_Commerce.Data;
using E_Commerce.Models;
using E_Commerce.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Controllers
{
    public class ManageController : Controller
    {
        private readonly AppDbContext _context;
        public ManageController(AppDbContext appDb)
        {
            _context = appDb;
        }

        private IActionResult? AuthorizeManager()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("SignIn", "Auth");

            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Manager")
                return RedirectToAction("Index", "Product");

            return null;
        }

        public IActionResult Index(string searchTerm, string sortOrder)
        {
            var authResult = AuthorizeManager();
            if (authResult != null) return authResult;

            var totalProducts = _context.Products.Count();
            var totalOrders = _context.Orders.Count();
            var totalRevenue = _context.Orders.Where(o => o.Status != "Cancelled").Sum(o => o.TotalPrice);
            var totalCustomers = _context.Users.Count(u => u.Role == "User");
            var pendingOrders = _context.Orders.Count(o => o.Status == "Pending");
            var lowStockProducts = _context.Products.Count(p => p.Quantity <= 5);
            var recentOrders = _context.Orders
                .Include(o => o.User)
                .OrderByDescending(o => o.dateTime)
                .Take(5)
                .ToList();

            ViewBag.TotalProducts = totalProducts;
            ViewBag.TotalOrders = totalOrders;
            ViewBag.TotalRevenue = totalRevenue;
            ViewBag.TotalCustomers = totalCustomers;
            ViewBag.PendingOrders = pendingOrders;
            ViewBag.LowStockProducts = lowStockProducts;
            ViewBag.RecentOrders = recentOrders;

            var products = _context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                products = products.Where(p =>
                    p.Name.Contains(searchTerm) ||
                    p.Description.Contains(searchTerm));
            }

            switch (sortOrder)
            {
                case "price-asc":
                    products = products.OrderBy(p => p.Price);
                    break;

                case "price-desc":
                    products = products.OrderByDescending(p => p.Price);
                    break;

                case "name":
                    products = products.OrderBy(p => p.Name);
                    break;

                case "stock":
                    products = products.OrderByDescending(p => p.Quantity);
                    break;

                default:
                    products = products.OrderByDescending(p => p.Id);
                    break;
            }
            return View(products.ToList());
        }

        [HttpGet]
        public IActionResult Add()
        {
            var authResult = AuthorizeManager();
            if (authResult != null) return authResult;

            ViewBag.Categories = new SelectList(_context.Categories.ToList(), "Id", "Name");
            return View("Add");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(ProductVM productVM)
        {
            var authResult = AuthorizeManager();
            if (authResult != null) return authResult;

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(_context.Categories.ToList(), "Id", "Name");
                return View("Add", productVM);
            }
            var product = new Product
            {
                Name = productVM.Name,
                Description = productVM.Description,
                ImageUrl = productVM.ImageUrl,
                Quantity = productVM.Quantity,
                Price = productVM.Price,
                DiscountPercent = productVM.DiscountPercent,
                SKU = productVM.SKU,
                Brand = productVM.Brand,
                CategoryId = productVM.CategoryId,
            };

            _context.Products.Add(product);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var authResult = AuthorizeManager();
            if (authResult != null) return authResult;

            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
                return NotFound();

            var productvm = new ProductVM()
            {
                Name = product.Name,
                Id = product.Id,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                Quantity = product.Quantity,
                Price = product.Price,
                DiscountPercent = product.DiscountPercent,
                SKU = product.SKU,
                Brand = product.Brand,
                CategoryId = product.CategoryId,
            };

            ViewBag.Categories = new SelectList(_context.Categories.ToList(), "Id", "Name");
            return View(productvm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ProductVM productVM, int id)
        {
            var authResult = AuthorizeManager();
            if (authResult != null) return authResult;

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(_context.Categories.ToList(), "Id", "Name");
                return View(productVM);
            }

            var existingProduct = _context.Products.FirstOrDefault(p => p.Id == id);
            if (existingProduct == null)
                return NotFound();

            existingProduct.Name = productVM.Name;
            existingProduct.Description = productVM.Description;
            existingProduct.ImageUrl = productVM.ImageUrl;
            existingProduct.Quantity = productVM.Quantity;
            existingProduct.Price = productVM.Price;
            existingProduct.DiscountPercent = productVM.DiscountPercent;
            existingProduct.SKU = productVM.SKU;
            existingProduct.Brand = productVM.Brand;
            existingProduct.CategoryId = productVM.CategoryId;

            _context.Products.Update(existingProduct);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            var authResult = AuthorizeManager();
            if (authResult != null) return authResult;

            if (id == null)
            {
                return NotFound();
            }

            var product = _context.Products
                .FirstOrDefault(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var authResult = AuthorizeManager();
            if (authResult != null) return authResult;

            var product = _context.Products.Find(id);

            if (product != null)
            {
                _context.Products.Remove(product);
            }

            _context.SaveChanges();

            return RedirectToAction("Index", "Manage");
        }

        // ================= ORDER MANAGEMENT =================

        public IActionResult Orders(string status, string searchTerm)
        {
            var authResult = AuthorizeManager();
            if (authResult != null) return authResult;

            var orders = _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status) && status != "all")
            {
                orders = orders.Where(o => o.Status == status);
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                orders = orders.Where(o =>
                    (o.FullName != null && o.FullName.Contains(searchTerm)) ||
                    (o.Phone != null && o.Phone.Contains(searchTerm)) ||
                    (o.City != null && o.City.Contains(searchTerm)) ||
                    o.Id.ToString() == searchTerm);
            }

            ViewBag.CurrentStatus = status ?? "all";
            ViewBag.SearchTerm = searchTerm;
            return View(orders.OrderByDescending(o => o.dateTime).ToList());
        }

        public IActionResult OrderDetails(int id)
        {
            var authResult = AuthorizeManager();
            if (authResult != null) return authResult;

            var order = _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Include(o => o.User)
                .FirstOrDefault(o => o.Id == id);

            if (order == null)
                return NotFound();

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateOrderStatus(int id, string status)
        {
            var authResult = AuthorizeManager();
            if (authResult != null) return authResult;

            var order = _context.Orders.Find(id);
            if (order == null)
                return NotFound();

            var validStatuses = new[] { "Pending", "Processing", "Shipped", "Delivered", "Cancelled" };
            if (!validStatuses.Contains(status))
                return BadRequest("Invalid status");

            if (status == "Cancelled" && order.Status != "Cancelled")
            {
                foreach (var item in _context.OrderItems.Where(oi => oi.OrderId == id).ToList())
                {
                    var product = _context.Products.Find(item.ProductId);
                    if (product != null)
                    {
                        product.Quantity += item.Quantity;
                    }
                }
            }

            order.Status = status;
            _context.SaveChanges();

            return RedirectToAction("OrderDetails", new { id });
        }
    }
}
