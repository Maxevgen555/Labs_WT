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
    public class EditModel : PageModel
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public EditModel(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        [BindProperty]
        public Dish Dish { get; set; } = default!;

        [BindProperty]
        public IFormFile? ImageFile { get; set; }

        public SelectList Categories { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var response = await _productService.GetProductByIdAsync(id.Value);
            if (!response.Success || response.Data == null)
            {
                return NotFound();
            }

            Dish = response.Data;
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

            // Если загружено новое изображение
            if (ImageFile != null && ImageFile.Length > 0)
            {
                // В реальном приложении нужно сохранять файл
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                Dish.Image = $"/images/dishes/{fileName}";
            }

            await _productService.UpdateProductAsync(Dish.Id, Dish, ImageFile);
            TempData["SuccessMessage"] = $"Блюдо \"{Dish.Name}\" успешно обновлено!";
            return RedirectToPage("./Index");
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
