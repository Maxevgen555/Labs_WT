using Labs.Domain.Entities;
using Labs.Domain.Models;
using Labs.UI.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Labs.UI.Areas.Admin.Pages.Dishes
{
    [Authorize(Policy = "admin")]
    public class IndexModel : PageModel
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public IndexModel(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        public List<Dish> Dishes { get; set; } = new();
        public string? CurrentCategory { get; set; }
        public List<Category> Categories { get; set; } = new();

        public async Task OnGetAsync(string? category)
        {
            // Получаем все категории для фильтра
            var categoriesResponse = await _categoryService.GetCategoryListAsync();
            if (categoriesResponse.Success)
            {
                Categories = categoriesResponse.Data ?? new();
            }

            // Получаем блюда с фильтрацией по категории
            // Используем pageSize = 100 для отображения всех блюд на одной странице
            var productResponse = await _productService.GetProductListAsync(category, 1, 100);
            if (productResponse.Success && productResponse.Data != null)
            {
                Dishes = productResponse.Data.Items ?? new();
            }

            CurrentCategory = category;
        }
    }
}
