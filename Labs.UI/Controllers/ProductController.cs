using Microsoft.AspNetCore.Mvc;
using Labs.UI.Services.Contracts;
using Labs.Domain.Entities;
using Microsoft.AspNetCore.Routing;

namespace Labs.UI.Controllers
{
    //[Route("Catalog")]
    //[Route("Catalog/{category?}")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public ProductController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index(string? category, int pageNo = 1)
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

            // Получение списка блюд с пагинацией
            var productResponse = await _productService.GetProductListAsync(category, pageNo);
            if (!productResponse.Success)
                ViewData["Error"] = productResponse.ErrorMessage;

            return View(productResponse.Data ?? new Domain.Models.ListModel<Dish>());
        }
    }
}