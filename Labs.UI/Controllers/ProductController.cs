using Microsoft.AspNetCore.Mvc;
using Labs.UI.Services.Contracts;
using Labs.Domain.Entities;

namespace Labs.UI.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public ProductController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index(string? category)
        {
            // Получение списка категорий
            var categoriesResponse = await _categoryService.GetCategoryListAsync();
            if (!categoriesResponse.Success)
                return NotFound(categoriesResponse.ErrorMessage);

            // Определение текущей категории
            var currentCategory = category == null
                ? "Все"
                : categoriesResponse.Data?.FirstOrDefault(c => c.NormalizedName == category)?.Name ?? "Все";

            ViewData["Categories"] = categoriesResponse.Data;
            ViewData["CurrentCategory"] = currentCategory;

            // Получение списка блюд
            var productResponse = await _productService.GetProductListAsync(category);
            if (!productResponse.Success)
                ViewData["Error"] = productResponse.ErrorMessage;

            return View(productResponse.Data?.Items ?? new List<Dish>());
        }
    }
}
