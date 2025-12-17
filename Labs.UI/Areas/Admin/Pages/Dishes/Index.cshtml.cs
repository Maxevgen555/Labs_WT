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

        public IndexModel(IProductService productService)
        {
            _productService = productService;
        }

        public List<Dish> Dish { get; set; } = new List<Dish>(); // Инициализируем пустым списком
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public string? ErrorMessage { get; set; }

        public async Task OnGetAsync(int? pageNo = 1)
        {
            try
            {
                var response = await _productService.GetProductListAsync(null, pageNo.Value);
                if (response.Success && response.Data != null)
                {
                    Dish = response.Data.Items ?? new List<Dish>();
                    CurrentPage = response.Data.CurrentPage;
                    TotalPages = response.Data.TotalPages;
                }
                else
                {
                    ErrorMessage = response.ErrorMessage ?? "Ошибка при загрузке данных";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка: {ex.Message}";
            }
        }
    }
}