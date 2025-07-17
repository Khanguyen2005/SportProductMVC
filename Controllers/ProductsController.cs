using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportProductMVC.Models;
using System.Globalization;

namespace SportProductMVC.Controllers
{
    public class ProductsController : Controller
    {
        private readonly SportProductContext _context;

        public ProductsController(SportProductContext context)
        {
            _context = context;
        }

        public IActionResult Details(int id)
        {
            var product = _context.Products.Find(id);

            if (product == null)
                return NotFound();

            return View(product);
        }

        public IActionResult Edit(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpPost]
        public IActionResult Edit(int id, Product model, IFormFile ProductImage)
        {
            if (id != model.ProductId)
                return BadRequest();

            // ✨ Xử lý Price từ Request.Form để chuyển dấu phẩy thành dấu chấm
            if (!string.IsNullOrWhiteSpace(Request.Form["Price"]))
            {
                var priceStr = Request.Form["Price"].ToString().Replace(",", ".");
                model.Price = decimal.TryParse(priceStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var value) ? value : null;
            }




            if (!ValidCategory(model.Category))
                ModelState.AddModelError("Category", "Danh mục không hợp lệ.");

            if (model.ManufacturingDate >= DateOnly.FromDateTime(DateTime.Today))
                ModelState.AddModelError("ManufacturingDate", "Ngày sản xuất phải nhỏ hơn ngày hiện tại.");

            var oldProduct = _context.Products.AsNoTracking().FirstOrDefault(p => p.ProductId == id);
            if (oldProduct == null) return NotFound();

            var oldPath = oldProduct.ImagePro;

            if (ModelState.IsValid)
            {
                if (ProductImage != null && ProductImage.Length > 0)
                {
                    var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                    Directory.CreateDirectory(uploadFolder);

                    var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(ProductImage.FileName).Replace(" ", "_")}";
                    var fullPath = Path.Combine(uploadFolder, fileName);

                    using var stream = new FileStream(fullPath, FileMode.Create);
                    ProductImage.CopyTo(stream);

                    // 🧼 Xóa file ảnh cũ nếu nó tồn tại
                    if (!string.IsNullOrEmpty(oldPath) && !oldPath.StartsWith("https://"))
                    {
                        var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", oldPath.TrimStart('/'));
                        if (System.IO.File.Exists(oldFilePath))
                            System.IO.File.Delete(oldFilePath);
                    }

                    model.ImagePro = "/images/" + fileName;
                }
                else
                {
                    // Không upload ảnh mới → giữ đường dẫn cũ
                    model.ImagePro = oldPath;
                }

                _context.Update(model);
                _context.SaveChanges();

                return RedirectToAction("Details", new { id = model.ProductId });
            }

            return View(model);
        }

        private bool ValidCategory(string category)
        {
            var validCategories = new[] { "Vợt", "Bóng", "Cầu", "Đệm", "Quần áo" };
            return validCategories.Contains(category.Trim());
        }

        public IActionResult ProductList()
        {
            var products = _context.Products.OrderBy(p => p.ProductId).ToList();
            return View(products);
        }
    }
}
