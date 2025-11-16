using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.EntityFrameworkCore;
using ShopCoffee.Attributes;
using ShopCoffee.Database;
using ShopCoffee.Helper; // cho phân trang
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;
using X.PagedList.Extensions;


namespace ShopCoffee.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Role("Administrator")]
    public class ProductAdminController : Controller
    {
        private readonly CoffeeShopContext _context;

        public ProductAdminController(CoffeeShopContext context)
        {
            _context = context;
        }

        // GET: Admin/ProductAdmin
        public async Task<IActionResult> Index(
             string searchString,
             int? categoryId,
             string sortOrder,
             int? page)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "name";
            ViewData["PriceSortParm"] = sortOrder == "Price" ? "price_desc" : "Price";
            ViewData["UpdateAtSortParm"] = sortOrder == "UpdateAt" ? "UpdateAt_desc" : "UpdateAt";
            ViewData["CreateAtSortParm"] = sortOrder == "CreateAt" ? "CreateAt_desc" : "CreateAt";
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentCategory"] = categoryId;


            var lsCate = await _context.Categories.ToListAsync();
            ViewData["Categories"] = lsCate;

            var products = _context.Products.Include(p => p.Category).AsQueryable();

            // --- Lọc theo tên ---
            if (!string.IsNullOrEmpty(searchString))
            {
                products = products.Where(p => p.Title.Contains(searchString));
            }

            // --- Lọc theo CategoryID ---
            if (categoryId.HasValue && categoryId.Value > 0)
            {
                products = products.Where(p => p.CategoryId == categoryId.Value);
            }

            // --- Sắp xếp ---
            switch (sortOrder)
            {
                case "namec":
                    products = products.OrderBy(p => p.Title);
                    break;
                case "name_desc":
                    products = products.OrderByDescending(p => p.Title);
                    break;
                case "Price":
                    products = products.OrderBy(p => p.Price);
                    break;
                case "price_desc":
                    products = products.OrderByDescending(p => p.Price);
                    break;
                case "UpdateAt":
                    products = products.OrderBy(p => p.UpdateAt);
                    break;
                case "UpdateAt_desc":
                    products = products 
                                .OrderByDescending(p => p.UpdateAt.HasValue)
                                .ThenByDescending(p => p.UpdateAt);       
                    break;
                case "CreateAt":
                    products = products.OrderBy(p => p.UpdateAt);
                    break;
                case "CreateAt_desc":
                    products = products.OrderByDescending(p => p.CreateAt);
                    break;

                default:
                    products = products.OrderByDescending(p => p.UpdateAt ?? p.CreateAt);
                    break;
            }

            int pageSize = 30;
            int pageNumber = (page ?? 1);

            return View(products.ToPagedList(pageNumber, pageSize));
        }



        // GET: Admin/ProductAdmin/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Admin/ProductAdmin/Create
        public IActionResult Create()
        {
            var model = new Product(); // khởi tạo trống
            model.Img = null;
            ViewBag.CategoryId = new SelectList(_context.Categories, "CategoryId", "Title");
            return View(model);
        }

        // POST: Admin/ProductAdmin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile fileImg)
        {
            //if (ModelState.IsValid)
            //{
            if (fileImg != null)
            {
                product.Img = await FileHelper.SaveImageAsync(fileImg, "products");
            }
            else
            {
                product.Img = Url.Content("~/images/placeholder.png");
            }

            product.CreateAt = DateTime.Now;
            _context.Add(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
            //}

            //ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Title", product.CategoryId);
            //return View(product);
        }

        // GET: Admin/ProductAdmin/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Title", product.CategoryId);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product, IFormFile fileImg)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            try
            {
                var now = DateTime.Now;
                product.UpdateAt = now;

                if (fileImg != null)
                {
                    product.Img = await FileHelper.SaveImageAsync(fileImg, "products");
                }
                else
                {
                    product.Img = Url.Content("~/images/placeholder.png");
                }

                _context.Update(product);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(product.ProductId))
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

        // GET: Admin/ProductAdmin/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Admin/ProductAdmin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
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
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
