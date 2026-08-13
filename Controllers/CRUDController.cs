using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using E_Commerce.Data;
using E_Commerce.Models;

namespace E_Commerce.Controllers
{
    public class CRUDController : Controller
    {
        private readonly AppDbContext _context;

        public CRUDController(AppDbContext context)
        {
            _context = context;
        }

        private bool IsManager()
        {
            return HttpContext.Session.GetString("UserRole") == "Manager";
        }

        private IActionResult? AuthorizeManager()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToAction("SignIn", "Auth");
            if (!IsManager())
                return RedirectToAction("Index", "Product");
            return null;
        }

        // GET: CRUD
        public IActionResult Index()
        {
            var auth = AuthorizeManager();
            if (auth != null) return auth;

            var productList = _context.Products.ToList();
            return View(productList);
        }


        // GET: CRUD/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var auth = AuthorizeManager();
            if (auth != null) return auth;

            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: CRUD/Create
        public IActionResult Create()
        {
            var auth = AuthorizeManager();
            if (auth != null) return auth;

            return View();
        }

        // POST: CRUD/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,Price,ImageUrl,Quantity")] Product product)
        {
            var auth = AuthorizeManager();
            if (auth != null) return auth;

            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: CRUD/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var auth = AuthorizeManager();
            if (auth != null) return auth;

            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: CRUD/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Price,ImageUrl,Quantity")] Product product)
        {
            var auth = AuthorizeManager();
            if (auth != null) return auth;

            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: CRUD/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var auth = AuthorizeManager();
            if (auth != null) return auth;

            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: CRUD/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var auth = AuthorizeManager();
            if (auth != null) return auth;

            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
