using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShopCoffee.Database;

namespace ShopCoffee.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PaymentDetailAdminController : Controller
    {
        private readonly CoffeeShopContext _context;

        public PaymentDetailAdminController(CoffeeShopContext context)
        {
            _context = context;
        }

        // GET: Admin/PaymentDetailAdmin
        public async Task<IActionResult> Index()
        {
            var coffeeShopContext = _context.PaymentDetails.Include(p => p.Payment).Include(p => p.Product);
            return View(await coffeeShopContext.ToListAsync());
        }

        // GET: Admin/PaymentDetailAdmin/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paymentDetail = await _context.PaymentDetails
                .Include(p => p.Payment)
                .Include(p => p.Product)
                .FirstOrDefaultAsync(m => m.PaymentDetailId == id);
            if (paymentDetail == null)
            {
                return NotFound();
            }

            return View(paymentDetail);
        }

        // GET: Admin/PaymentDetailAdmin/Create
        public IActionResult Create()
        {
            ViewData["PaymentId"] = new SelectList(_context.Payments, "PaymentId", "PaymentId");
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductId");
            return View();
        }

        // POST: Admin/PaymentDetailAdmin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PaymentDetailId,ProductId,PaymentId,Price,Quantity,Total,CreateAt")] PaymentDetail paymentDetail)
        {
            if (ModelState.IsValid)
            {
                _context.Add(paymentDetail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PaymentId"] = new SelectList(_context.Payments, "PaymentId", "PaymentId", paymentDetail.PaymentId);
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductId", paymentDetail.ProductId);
            return View(paymentDetail);
        }

        // GET: Admin/PaymentDetailAdmin/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paymentDetail = await _context.PaymentDetails.FindAsync(id);
            if (paymentDetail == null)
            {
                return NotFound();
            }
            ViewData["PaymentId"] = new SelectList(_context.Payments, "PaymentId", "PaymentId", paymentDetail.PaymentId);
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductId", paymentDetail.ProductId);
            return View(paymentDetail);
        }

        // POST: Admin/PaymentDetailAdmin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PaymentDetailId,ProductId,PaymentId,Price,Quantity,Total,CreateAt")] PaymentDetail paymentDetail)
        {
            if (id != paymentDetail.PaymentDetailId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(paymentDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaymentDetailExists(paymentDetail.PaymentDetailId))
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
            ViewData["PaymentId"] = new SelectList(_context.Payments, "PaymentId", "PaymentId", paymentDetail.PaymentId);
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductId", paymentDetail.ProductId);
            return View(paymentDetail);
        }

        // GET: Admin/PaymentDetailAdmin/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paymentDetail = await _context.PaymentDetails
                .Include(p => p.Payment)
                .Include(p => p.Product)
                .FirstOrDefaultAsync(m => m.PaymentDetailId == id);
            if (paymentDetail == null)
            {
                return NotFound();
            }

            return View(paymentDetail);
        }

        // POST: Admin/PaymentDetailAdmin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var paymentDetail = await _context.PaymentDetails.FindAsync(id);
            if (paymentDetail != null)
            {
                _context.PaymentDetails.Remove(paymentDetail);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PaymentDetailExists(int id)
        {
            return _context.PaymentDetails.Any(e => e.PaymentDetailId == id);
        }
    }
}
