using Labs.Domain.Entities;
using Labs.Domain.Models;
using Labs.UI.Services.Contracts;
using Microsoft.AspNetCore.Http;

namespace Labs.UI.Services
{
    public class MemoryProductService : IProductService
    {
        private readonly List<Dish> _dishes;
        private readonly List<Category> _categories;

        public MemoryProductService(ICategoryService categoryService)
        {
            var categoriesResponse = categoryService.GetCategoryListAsync().Result;
            _categories = categoriesResponse.Data ?? new List<Category>();
            _dishes = new List<Dish>();
            SetupData();
        }

        private void SetupData()
        {
            _dishes.AddRange(new List<Dish>
            {
                new Dish
                {
                    Id = 1,
                    Name = "Суп-харчо",
                    Description = "Очень острый, невкусный",
                    Calories = 200,
                    Image = "/images/soup.jpg",
                    CategoryId = _categories.Find(c => c.NormalizedName == "soups")?.Id ?? 3,
                    Category = _categories.Find(c => c.NormalizedName == "soups")
                },
                new Dish
                {
                    Id = 2,
                    Name = "Борщ",
                    Description = "Много сала, без сметаны",
                    Calories = 330,
                    Image = "/images/borshch.jpg",
                    CategoryId = _categories.Find(c => c.NormalizedName == "soups")?.Id ?? 3,
                    Category = _categories.Find(c => c.NormalizedName == "soups")
                },
                new Dish
                {
                    Id = 3,
                    Name = "Цезарь",
                    Description = "С курицей и пармезаном",
                    Calories = 450,
                    Image = "/images/salad.jpg",
                    CategoryId = _categories.Find(c => c.NormalizedName == "salads")?.Id ?? 2,
                    Category = _categories.Find(c => c.NormalizedName == "salads")
                },
                new Dish
                {
                    Id = 4,
                    Name = "Стейк",
                    Description = "Мраморная говядина",
                    Calories = 800,
                    Image = "/images/steak.jpg",
                    CategoryId = _categories.Find(c => c.NormalizedName == "main")?.Id ?? 4,
                    Category = _categories.Find(c => c.NormalizedName == "main")
                }
            });
        }

        public Task<ResponseData<ListModel<Dish>>> GetProductListAsync(string? categoryNormalizedName, int pageNo = 1)
        {
            var result = new ResponseData<ListModel<Dish>>();

            int? categoryId = null;
            if (categoryNormalizedName != null)
            {
                categoryId = _categories.Find(c => c.NormalizedName.Equals(categoryNormalizedName))?.Id;
            }

            var data = _dishes
                .Where(d => categoryId == null || d.CategoryId == categoryId)
                .ToList();

            result.Data = new ListModel<Dish>
            {
                Items = data,
                CurrentPage = pageNo,
                TotalPages = (int)Math.Ceiling(data.Count / 6.0)
            };

            if (data.Count == 0)
            {
                result.Success = false;
                result.ErrorMessage = "Нет объектов в выбранной категории";
            }

            return Task.FromResult(result);
        }

        public Task<ResponseData<Dish>> GetProductByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateProductAsync(int id, Dish product, IFormFile? formFile)
        {
            throw new NotImplementedException();
        }

        public Task DeleteProductAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseData<Dish>> CreateProductAsync(Dish product, IFormFile? formFile)
        {
            throw new NotImplementedException();
        }

        // ... остальные методы IProductService ...
    }
}
