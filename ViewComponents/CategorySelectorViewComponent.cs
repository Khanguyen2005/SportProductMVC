using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SportProductMVC.ViewComponents
{
    public class CategorySelectorViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string selectedCategory)
        {
            var categories = new[] { "Vợt", "Bóng", "Cầu", "Đệm", "Quần áo" };

            var list = categories.Select(c => new SelectListItem
            {
                Text = c,
                Value = c,
                Selected = c == selectedCategory
            });

            return View(list);
        }
    }
}