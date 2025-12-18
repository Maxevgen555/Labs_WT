using Labs.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Labs.API.Data
{
    public static class DbInitializer
    {
        public static async Task SeedData(WebApplication app)
        {
            try
            {
                using var scope = app.Services.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                // Очищаем и создаем базу данных заново
                await context.Database.EnsureDeletedAsync();
                await context.Database.EnsureCreatedAsync();

                var categories = new List<Category>
                {
                    new Category { Name = "Стартеры", NormalizedName = "starters" },
                    new Category { Name = "Салаты", NormalizedName = "salads" },
                    new Category { Name = "Супы", NormalizedName = "soups" },
                    new Category { Name = "Основные блюда", NormalizedName = "main" },
                    new Category { Name = "Напитки", NormalizedName = "drinks" },
                    new Category { Name = "Десерты", NormalizedName = "desserts" }
                };

                await context.Categories.AddRangeAsync(categories);
                await context.SaveChangesAsync();

                // Создаем блюда
                var dishes = new List<Dish>
                {
                    new Dish
                    {
                        Name = "Суп-харчо",
                        Description = "Очень острый, невкусный",
                        Calories = 200,
                        Image = "/images/soup.jpg",
                        CategoryId = categories.FirstOrDefault(c => c.NormalizedName == "soups")?.Id ?? 3
                    },
                    new Dish
                    {
                        Name = "Борщ",
                        Description = "Много сала, без сметаны",
                        Calories = 330,
                        Image = "/images/borshch.jpg",
                        CategoryId = categories.FirstOrDefault(c => c.NormalizedName == "soups")?.Id ?? 3
                    },
                    new Dish
                    {
                        Name = "Цезарь",
                        Description = "С курицей и пармезаном",
                        Calories = 450,
                        Image = "/images/salad.jpg",
                        CategoryId = categories.FirstOrDefault(c => c.NormalizedName == "salads")?.Id ?? 2
                    },
                    // ... остальные блюда
                };

                await context.Dishes.AddRangeAsync(dishes);
                await context.SaveChangesAsync();

                Console.WriteLine("База данных успешно инициализирована!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка инициализации базы данных: {ex.Message}");
            }
        }
    }
}