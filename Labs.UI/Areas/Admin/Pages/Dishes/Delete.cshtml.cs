using Labs.Domain.Entities;
using Labs.Domain.Models;
using Labs.UI.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Labs.UI.Areas.Admin.Pages.Dishes
{
    [Authorize(Policy = "admin")]
    public class DeleteModel : PageModel
    {
        private readonly IProductService _productService;

        public DeleteModel(IProductService productService)
        {
            _productService = productService;
        }

        public Dish Dish { get; set; } = default!;

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
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Получаем блюдо для отображения имени в сообщении
            var response = await _productService.GetProductByIdAsync(id.Value);
            if (!response.Success || response.Data == null)
            {
                return NotFound();
            }

            var dishName = response.Data.Name;
            await _productService.DeleteProductAsync(id.Value);

            TempData["SuccessMessage"] = $"Блюдо \"{dishName}\" успешно удалено!";
            return RedirectToPage("./Index");
        }
    }
}
