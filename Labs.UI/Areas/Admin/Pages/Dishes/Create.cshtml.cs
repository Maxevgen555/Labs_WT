using Labs.Domain.Entities;
using Labs.Domain.Models;
using Labs.UI.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Labs.UI.Areas.Admin.Pages.Dishes
{
    [Authorize(Policy = "admin")]
    public class CreateModel : PageModel
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public CreateModel(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        [BindProperty]
        public Dish Dish { get; set; } = new();

        [BindProperty]
        public IFormFile? ImageFile { get; set; }

        public SelectList Categories { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadCategories();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadCategories();
                return Page();
            }

            // Если загружено изображение, обрабатываем его
            if (ImageFile != null && ImageFile.Length > 0)
            {
                // Здесь можно сохранить файл, но пока просто укажем путь
                // В реальном приложении нужно сохранять файл в wwwroot/images
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                Dish.Image = $"/images/dishes/{fileName}";
            }

            var response = await _productService.CreateProductAsync(Dish, ImageFile);

            if (response.Success)
            {
                TempData["SuccessMessage"] = $"Блюдо \"{Dish.Name}\" успешно создано!";
                return RedirectToPage("./Index");
            }

            ModelState.AddModelError(string.Empty, response.ErrorMessage ?? "Ошибка при создании блюда");
            await LoadCategories();
            return Page();
        }

        private async Task LoadCategories()
        {
            var categoriesResponse = await _categoryService.GetCategoryListAsync();
            if (categoriesResponse.Success && categoriesResponse.Data != null)
            {
                Categories = new SelectList(categoriesResponse.Data, "Id", "Name");
            }
            else
            {
                Categories = new SelectList(new List<Category>(), "Id", "Name");
            }
        }
    }
}
