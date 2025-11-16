using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.EntityFrameworkCore;
using ShopCoffee.Attributes;
using ShopCoffee.Database;
using ShopCoffee.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopCoffee.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Role("Administrator")]
    public class CustomerAdminController : Controller
    {
        private readonly CoffeeShopContext _context;

        public CustomerAdminController(CoffeeShopContext context)
        {
            _context = context;
        }

        // GET: Admin/CustomerAdmin
        public async Task<IActionResult> Index()
        {
            return View(await _context.Customers.ToListAsync());
        }

        // GET: Admin/CustomerAdmin/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // GET: Admin/CustomerAdmin/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/CustomerAdmin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Customer model, IFormFile? Img)
        {
            if (Img != null)
            {
                model.Img = await FileHelper.SaveImageAsync(Img, "customer");
            }
            else
            {
                model.Img = Url.Content("~/images/placeholder.png");
            }

            if (model.Password != null)
            {
                model.RandomKey = PasswordHelper.GenerateRandomKey();
                model.Password = model.Password.ToMd5Hash(model.RandomKey);
            }

            model.RegisteredAt = DateTime.Now;
            _context.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/CustomerAdmin/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // POST: Admin/CustomerAdmin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Customer model, IFormFile? Img)
        {
            if (Img != null)
            {
                model.Img = await FileHelper.SaveImageAsync(Img, "customer");
            }
            else
            {
                model.Img = Url.Content("~/images/placeholder.png");
            }

            if (model.Password != null)
            {
                model.RandomKey = PasswordHelper.GenerateRandomKey();
                model.Password = model.Password.ToMd5Hash(model.RandomKey);
            }

            model.UpdateAt = DateTime.Now;
            _context.Update(model);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/CustomerAdmin/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Admin/CustomerAdmin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.CustomerId == id);
        }
    }
}
