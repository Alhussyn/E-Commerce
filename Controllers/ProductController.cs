using Microsoft.AspNetCore.Mvc;
using E_Commerce.Models;
using System.Collections.Generic;
using E_Commerce.Data;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Controllers
{
    public class ProductController : Controller
    {
       private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
            
        }

        public IActionResult index()
        {
            var products=_context.Products.ToList();
            return View(products);
        }

        public IActionResult Details(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id== id);
            if(product==null)
            {
                return NotFound();
            }

            return View(product);
        }

        public IActionResult AddToCart(int id)
        {
            //  هات أي user (مؤقت)
            var user = _context.Users.FirstOrDefault();

            if (user == null)
                return Content("No users found");

            int userId = user.Id;

            //  تأكد إن المنتج موجود
            var product = _context.Products.Find(id);
            if (product == null)
                return NotFound();

            //  هات الكارت
            var cart = _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefault(c => c.UserId == userId);

            // لو مفيش كارت → اعمله
            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                _context.Carts.Add(cart);
            }

            //  هل المنتج موجود؟
            var item = cart.CartItems
                .FirstOrDefault(c => c.ProductId == id);

            if (item != null)
            {
                item.Quantity++;
            }
            else
            {
                cart.CartItems.Add(new CartItem
                {
                    ProductId = id,
                    Quantity = 1
                });
            }

            //  احفظ
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Cart()
        {
            var user = _context.Users.FirstOrDefault();

            if (user == null)
                return Content("No user found");

            var cart = _context.Carts
                .Where(c => c.UserId == user.Id)
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefault();

            return View(cart);
        }


        [HttpPost]
        public IActionResult ClearCart()
        {
            var items = _context.CartItems.ToList();

            _context.CartItems.RemoveRange(items);
            _context.SaveChanges();

            return View("Cart");
        }

        public IActionResult Buy(int id)
        {
            // search on products
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if(product==null)
            {
                return NotFound();
            }
            //user
            var user=_context.Users.FirstOrDefault(U=>U.Id==1);
            if(user==null)
            {
                return NotFound();
            }
            // prepare order item
            var item = new OrderItem();
            item.ProductId = product.Id;
            item.Quantity = 1;
            item.Price = product.Price;

            //prepare order

            var order=new Order();
            order.TotalPrice=product.Price;
            order.UserId=user.Id;
            // الربط علشان ننشئ اول order 
            order.OrderItems=new List<OrderItem>();
            order.OrderItems.Add(item);
            // نسمع في ال database
            _context.Orders.Add(order);
            _context.SaveChanges();

            return View("Confirm");

        }

        public IActionResult auth()
        {
            return View("auth");
        }
            


























    }
}