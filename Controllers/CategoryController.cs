using E_Commerce.Data;
using E_Commerce.Models;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Controllers
{
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        private IActionResult? AuthorizeManager()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("SignIn", "Auth");

            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Manager")
                return Forbid();

            return null;
        }

        public IActionResult Index()
        {
            var authResult = AuthorizeManager();
            if (authResult != null) return authResult;

            var categories = _context.Categories.ToList();
            return View(categories);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var authResult = AuthorizeManager();
            if (authResult != null) return authResult;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            var authResult = AuthorizeManager();
            if (authResult != null) return authResult;

            if (!ModelState.IsValid)
                return View(category);

            if (_context.Categories.Any(c => c.Name == category.Name))
            {
                ModelState.AddModelError("Name", "A category with this name already exists.");
                return View(category);
            }

            _context.Categories.Add(category);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var authResult = AuthorizeManager();
            if (authResult != null) return authResult;

            var category = _context.Categories.Find(id);
            if (category == null)
                return NotFound();

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Category category)
        {
            var authResult = AuthorizeManager();
            if (authResult != null) return authResult;

            if (!ModelState.IsValid)
                return View(category);

            var existing = _context.Categories.Find(id);
            if (existing == null)
                return NotFound();

            if (_context.Categories.Any(c => c.Name == category.Name && c.Id != id))
            {
                ModelState.AddModelError("Name", "A category with this name already exists.");
                return View(category);
            }

            existing.Name = category.Name;
            existing.Description = category.Description;
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var authResult = AuthorizeManager();
            if (authResult != null) return authResult;

            var category = _context.Categories.Find(id);
            if (category == null)
                return NotFound();

            var productsInCategory = _context.Products.Where(p => p.CategoryId == id).ToList();
            foreach (var product in productsInCategory)
            {
                product.CategoryId = null;
            }

            _context.Categories.Remove(category);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
