using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopCoffee.Database;
using X.PagedList.Extensions;

namespace ShopCoffee.Controllers
{
    public class ProductController : Controller
    {
        private readonly CoffeeShopContext _context;

        public ProductController(CoffeeShopContext context)
        {
            _context = context;
        }
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

        public async Task<IActionResult> Detail(int? id)
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
    }
}
