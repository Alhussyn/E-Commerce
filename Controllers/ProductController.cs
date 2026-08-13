using E_Commerce.Data;
using E_Commerce.Models;
using E_Commerce.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private const int PageSize = 9;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string searchTerm, string sortOrder, int? categoryId, int page = 1)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("SignIn", "Auth");

            var products = _context.Products
                .Include(p => p.Category)
                .AsQueryable();

            if (categoryId.HasValue)
            {
                products = products.Where(p => p.CategoryId == categoryId.Value);
                ViewBag.SelectedCategory = categoryId.Value;
                ViewBag.SelectedCategoryName = _context.Categories.Find(categoryId.Value)?.Name;
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                products = products.Where(p =>
                    p.Name.Contains(searchTerm) ||
                    p.Description.Contains(searchTerm) ||
                    (p.Brand != null && p.Brand.Contains(searchTerm)) ||
                    (p.Category != null && p.Category.Name.Contains(searchTerm)));
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

            var totalItems = products.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)PageSize);
            page = Math.Max(1, Math.Min(page, Math.Max(1, totalPages)));

            var pagedProducts = products
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.SearchTerm = searchTerm;
            ViewBag.SortOrder = sortOrder;
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.CartCount = _context.CartItems
                .Where(item => item.Cart!.UserId == userId.Value)
                .Sum(item => (int?)item.Quantity) ?? 0;

            return View(pagedProducts);
        }

        public IActionResult Details(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("SignIn", "Auth");

            var product = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .FirstOrDefault(p => p.Id == id);

            if (product == null)
                return NotFound();

            var reviews = _context.Reviews
                .Include(r => r.User)
                .Where(r => r.ProductId == id)
                .OrderByDescending(r => r.CreatedAt)
                .ToList();

            var avgRating = reviews.Any() ? reviews.Average(r => r.Rating) : 0;
            var isInWishlist = _context.Wishlists.Any(w => w.UserId == userId.Value && w.ProductId == id);
            var relatedProducts = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Where(p => p.CategoryId == product.CategoryId && p.Id != id)
                .Take(4)
                .ToList();

            ViewBag.Reviews = reviews;
            ViewBag.AvgRating = Math.Round(avgRating, 1);
            ViewBag.ReviewCount = reviews.Count;
            ViewBag.IsInWishlist = isInWishlist;
            ViewBag.RelatedProducts = relatedProducts;

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddToCart(int id, int quantity = 1)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("SignIn", "Auth");

            quantity = Math.Max(1, quantity);
            var product = _context.Products.Find(id);
            if (product == null)
                return NotFound();

            var cart = _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefault(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId.Value };
                _context.Carts.Add(cart);
            }

            var item = cart.CartItems.FirstOrDefault(c => c.ProductId == id);
            var currentCartQuantity = item?.Quantity ?? 0;

            if (currentCartQuantity + quantity > product.Quantity)
            {
                if (IsAjaxRequest())
                    return Json(new { success = false, message = $"Only {product.Quantity} available in stock." });
                TempData["CartMessage"] = $"Insufficient stock. Only {product.Quantity} available.";
                return RedirectToAction("Details", new { id });
            }

            if (item != null)
                item.Quantity += quantity;
            else
                cart.CartItems.Add(new CartItem { ProductId = id, Quantity = quantity });

            _context.SaveChanges();
            if (IsAjaxRequest())
                return CartResponse(cart, "Product added to cart.");
            TempData["CartMessage"] = "Product added to cart successfully";
            return RedirectToAction("Details", new { id });
        }

        public IActionResult Cart()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("SignIn", "Auth");

            var cart = _context.Carts
                .Where(c => c.UserId == userId)
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefault();

            return View(cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ClearCart()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("SignIn", "Auth");

            var cart = _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefault(c => c.UserId == userId);

            if (cart != null && cart.CartItems.Any())
            {
                _context.CartItems.RemoveRange(cart.CartItems);
                cart.CartItems.Clear();
                _context.SaveChanges();
            }

            if (IsAjaxRequest())
                return Json(new { success = true, message = "Cart cleared.", cartCount = 0, subtotal = 0m, cartEmpty = true });

            return RedirectToAction("Cart");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveCartItem(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("SignIn", "Auth");

            var cart = _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefault(c => c.UserId == userId);

            if (cart == null)
                return RedirectToAction("Cart");

            var item = cart.CartItems.FirstOrDefault(ci => ci.Id == id);
            if (item != null)
            {
                _context.CartItems.Remove(item);
                cart.CartItems.Remove(item);
                _context.SaveChanges();
            }

            if (IsAjaxRequest())
                return CartResponse(cart, "Item removed from cart.");

            return RedirectToAction("Cart");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateQuantity(int id, int quantity)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("SignIn", "Auth");

            var cart = _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefault(c => c.UserId == userId);

            if (cart == null)
                return RedirectToAction("Cart");

            var item = cart.CartItems.FirstOrDefault(ci => ci.Id == id);
            if (item != null && item.Product != null)
            {
                if (quantity <= 0)
                {
                    _context.CartItems.Remove(item);
                    cart.CartItems.Remove(item);
                }
                else if (quantity <= item.Product.Quantity)
                    item.Quantity = quantity;
                else
                    TempData["CartMessage"] = $"Only {item.Product.Quantity} available in stock.";

                _context.SaveChanges();
            }

            if (IsAjaxRequest())
            {
                if (item == null) return Json(new { success = false, message = "Cart item was not found." });
                return CartResponse(cart, quantity <= 0 ? "Item removed from cart." : "Cart updated.");
            }

            return RedirectToAction("Cart");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Buy(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("SignIn", "Auth");

            var product = _context.Products.Find(id);
            if (product == null)
                return NotFound();

            if (product.Quantity <= 0)
            {
                TempData["CartMessage"] = "This product is out of stock.";
                return RedirectToAction("Details", new { id });
            }

            var cart = _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefault(c => c.UserId == userId.Value);

            if (cart == null)
            {
                cart = new Cart { UserId = userId.Value, CartItems = new List<CartItem>() };
                _context.Carts.Add(cart);
            }

            var item = cart.CartItems.FirstOrDefault(c => c.ProductId == id);
            var currentCartQuantity = item?.Quantity ?? 0;

            if (currentCartQuantity + 1 > product.Quantity)
            {
                TempData["CartMessage"] = $"Insufficient stock. Only {product.Quantity} available.";
                return RedirectToAction("Cart");
            }

            if (item != null)
                item.Quantity++;
            else
                cart.CartItems.Add(new CartItem { ProductId = id, Quantity = 1 });

            _context.SaveChanges();
            return RedirectToAction("Cart");
        }

        // ================= WISHLIST =================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleWishlist(int productId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("SignIn", "Auth");

            var existing = _context.Wishlists
                .FirstOrDefault(w => w.UserId == userId.Value && w.ProductId == productId);

            if (existing != null)
            {
                _context.Wishlists.Remove(existing);
                TempData["CartMessage"] = "Removed from wishlist";
            }
            else
            {
                _context.Wishlists.Add(new Wishlist
                {
                    UserId = userId.Value,
                    ProductId = productId
                });
                TempData["CartMessage"] = "Added to wishlist";
            }

            _context.SaveChanges();
            return RedirectToAction("Details", new { id = productId });
        }

        public IActionResult Wishlist()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("SignIn", "Auth");

            var items = _context.Wishlists
                .Where(w => w.UserId == userId.Value)
                .Include(w => w.Product)
                .ThenInclude(p => p!.Category)
                .OrderByDescending(w => w.CreatedAt)
                .ToList();

            return View(items);
        }

        // ================= REVIEWS =================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddReview(int productId, int rating, string comment)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("SignIn", "Auth");

            if (rating < 1 || rating > 5)
            {
                TempData["CartMessage"] = "Rating must be between 1 and 5.";
                return RedirectToAction("Details", new { id = productId });
            }

            var existingReview = _context.Reviews
                .FirstOrDefault(r => r.UserId == userId.Value && r.ProductId == productId);

            if (existingReview != null)
            {
                existingReview.Rating = rating;
                existingReview.Comment = comment ?? string.Empty;
                existingReview.CreatedAt = DateTime.UtcNow;
            }
            else
            {
                _context.Reviews.Add(new Review
                {
                    UserId = userId.Value,
                    ProductId = productId,
                    Rating = rating,
                    Comment = comment ?? string.Empty
                });
            }

            _context.SaveChanges();
            TempData["CartMessage"] = "Review submitted successfully";
            return RedirectToAction("Details", new { id = productId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteReview(int reviewId, int productId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("SignIn", "Auth");

            var review = _context.Reviews.FirstOrDefault(r => r.Id == reviewId && r.UserId == userId.Value);
            if (review != null)
            {
                _context.Reviews.Remove(review);
                _context.SaveChanges();
                TempData["CartMessage"] = "Review deleted";
            }

            return RedirectToAction("Details", new { id = productId });
        }

        // ================= CHECKOUT =================

        [HttpGet]
        public IActionResult Checkout()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("SignIn", "Auth");

            var cart = _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefault(c => c.UserId == userId.Value);

            if (cart == null || !cart.CartItems.Any())
                return RedirectToAction("Cart");

            var vm = new CheckoutViewModel
            {
                CartItems = cart.CartItems
                    .Where(ci => ci.Product != null)
                    .Select(ci => new CartItemViewModel
                    {
                        ProductId = ci.ProductId,
                        ProductName = ci.Product!.Name,
                        ProductImageUrl = ci.Product.ImageUrl,
                        Quantity = ci.Quantity,
                        UnitPrice = ci.Product.FinalPrice
                    }).ToList(),
                TotalPrice = cart.CartItems
                    .Where(i => i.Product != null)
                    .Sum(i => i.Quantity * i.Product!.FinalPrice)
            };

            return View("Confirm", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Checkout(CheckoutViewModel model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("SignIn", "Auth");

            if (!ModelState.IsValid)
            {
                var cartForDisplay = _context.Carts
                    .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                    .FirstOrDefault(c => c.UserId == userId.Value);

                if (cartForDisplay == null || !cartForDisplay.CartItems.Any())
                    return RedirectToAction("Cart");

                model.CartItems = cartForDisplay.CartItems
                    .Where(ci => ci.Product != null)
                    .Select(ci => new CartItemViewModel
                    {
                        ProductId = ci.ProductId,
                        ProductName = ci.Product!.Name,
                        ProductImageUrl = ci.Product.ImageUrl,
                        Quantity = ci.Quantity,
                        UnitPrice = ci.Product.FinalPrice
                    }).ToList();
                model.TotalPrice = cartForDisplay.CartItems
                    .Where(i => i.Product != null)
                    .Sum(i => i.Quantity * i.Product!.FinalPrice);

                return View("Confirm", model);
            }

            var cart = _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefault(c => c.UserId == userId.Value);

            if (cart == null || !cart.CartItems.Any())
                return RedirectToAction("Cart");

            foreach (var cartItem in cart.CartItems)
            {
                var dbProduct = _context.Products.Find(cartItem.ProductId);
                if (dbProduct == null || cartItem.Quantity > dbProduct.Quantity)
                {
                    var productName = cartItem.Product?.Name ?? "Unknown";
                    var availableQty = dbProduct?.Quantity ?? 0;
                    TempData["CartMessage"] = $"Insufficient stock for {productName}. Only {availableQty} available.";
                    return RedirectToAction("Cart");
                }
            }

            using var transaction = _context.Database.BeginTransaction(System.Data.IsolationLevel.Serializable);
            try
            {
                var order = new Order
                {
                    UserId = userId.Value,
                    FullName = model.FullName,
                    Phone = model.Phone,
                    Address = model.Address,
                    City = model.City,
                    Notes = model.Notes,
                    DeliveryDate = model.DeliveryDate,
                    dateTime = DateTime.UtcNow,
                    OrderItems = new List<OrderItem>()
                };

                foreach (var cartItem in cart.CartItems)
                {
                    var dbProduct = _context.Products.Find(cartItem.ProductId);
                    if (dbProduct == null || cartItem.Quantity > dbProduct.Quantity)
                    {
                        transaction.Rollback();
                        var pName = cartItem.Product?.Name ?? "Unknown";
                        TempData["CartMessage"] = $"Insufficient stock for {pName}. Please update your cart.";
                        return RedirectToAction("Cart");
                    }
                    dbProduct.Quantity -= cartItem.Quantity;

                    order.OrderItems.Add(new OrderItem
                    {
                        ProductId = cartItem.ProductId,
                        ProductName = cartItem.Product?.Name ?? "Unknown",
                        Quantity = cartItem.Quantity,
                        Price = dbProduct.FinalPrice
                    });
                }

                order.TotalPrice = order.OrderItems.Sum(i => i.Quantity * i.Price);

                _context.Orders.Add(order);
                _context.CartItems.RemoveRange(cart.CartItems);

                _context.SaveChanges();
                transaction.Commit();

                TempData["CartMessage"] = "Order confirmed successfully";
                return RedirectToAction("Details", "Order", new { id = order.Id });
            }
            catch
            {
                transaction.Rollback();
                TempData["CartMessage"] = "An error occurred while processing your order. Please try again.";
                return RedirectToAction("Cart");
            }
        }

        public IActionResult auth()
        {
            return View("auth");
        }

        private bool IsAjaxRequest() => Request.Headers["X-Requested-With"] == "XMLHttpRequest";

        private JsonResult CartResponse(Cart? cart, string message)
        {
            var items = cart?.CartItems ?? new List<CartItem>();
            var subtotal = items.Where(item => item.Product != null)
                .Sum(item => item.Quantity * item.Product!.FinalPrice);
            return Json(new
            {
                success = true,
                message,
                cartCount = items.Sum(item => item.Quantity),
                subtotal,
                cartEmpty = !items.Any()
            });
        }
    }
}
