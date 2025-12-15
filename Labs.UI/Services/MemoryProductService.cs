using Labs.Domain.Entities;
using Labs.Domain.Models;
using Labs.UI.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Labs.UI.Services
{
    public class MemoryProductService : IProductService
    {
        private readonly List<Dish> _dishes;
        private readonly List<Category> _categories;
        private readonly int _pageSize;

        public MemoryProductService(IConfiguration config, ICategoryService categoryService)
        {
            _pageSize = config.GetValue<int>("ItemsPerPage", 3);

            var categoriesResponse = categoryService.GetCategoryListAsync().Result;
            _categories = categoriesResponse.Data ?? new List<Category>();
            _dishes = new List<Dish>();
            SetupData();
        }

        private void SetupData()
        {
            // Добавим больше тестовых данных для демонстрации пагинации
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
                },
                new Dish
                {
                    Id = 5,
                    Name = "Греческий салат",
                    Description = "С фетой и оливками",
                    Calories = 350,
                    Image = "/images/salad2.jpg",
                    CategoryId = _categories.Find(c => c.NormalizedName == "salads")?.Id ?? 2,
                    Category = _categories.Find(c => c.NormalizedName == "salads")
                },
                new Dish
                {
                    Id = 6,
                    Name = "Куриный суп",
                    Description = "Домашний, с лапшой",
                    Calories = 280,
                    Image = "/images/soup2.jpg",
                    CategoryId = _categories.Find(c => c.NormalizedName == "soups")?.Id ?? 3,
                    Category = _categories.Find(c => c.NormalizedName == "soups")
                },
                new Dish
                {
                    Id = 7,
                    Name = "Лазанья",
                    Description = "Итальянская классика",
                    Calories = 650,
                    Image = "/images/main2.jpg",
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

            // Вычисляем общее количество страниц
            int totalPages = (int)Math.Ceiling(data.Count / (double)_pageSize);

            // Применяем пагинацию
            var pagedData = data
                .Skip((pageNo - 1) * _pageSize)
                .Take(_pageSize)
                .ToList();

            result.Data = new ListModel<Dish>
            {
                Items = pagedData,
                CurrentPage = pageNo,
                TotalPages = totalPages
            };

            if (data.Count == 0)
            {
                result.Success = false;
                result.ErrorMessage = "Нет объектов в выбранной категории";
            }

            return Task.FromResult(result);
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

        public Task<ResponseData<Dish>> GetProductByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
