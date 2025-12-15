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

                // Выполнить миграции
                await context.Database.MigrateAsync();

                // Если база уже заполнена, выходим
                if (await context.Categories.AnyAsync() || await context.Dishes.AnyAsync())
                {
                    return;
                }

                var apiUrl = "https://localhost:7002";

                // Создаем категории
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
                        Image = $"{apiUrl}/images/soup.jpg",
                        Category = categories.FirstOrDefault(c => c.NormalizedName == "soups")
                    },
                    new Dish
                    {
                        Name = "Борщ",
                        Description = "Много сала, без сметаны",
                        Calories = 330,
                        Image = $"{apiUrl}/images/borshch.jpg",
                        Category = categories.FirstOrDefault(c => c.NormalizedName == "soups")
                    },
                    new Dish
                    {
                        Name = "Цезарь",
                        Description = "С курицей и пармезаном",
                        Calories = 450,
                        Image = $"{apiUrl}/images/salad.jpg",
                        Category = categories.FirstOrDefault(c => c.NormalizedName == "salads")
                    },
                    new Dish
                    {
                        Name = "Стейк",
                        Description = "Мраморная говядина",
                        Calories = 800,
                        Image = $"{apiUrl}/images/steak.jpg",
                        Category = categories.FirstOrDefault(c => c.NormalizedName == "main")
                    },
                    new Dish
                    {
                        Name = "Греческий салат",
                        Description = "С фетой и оливками",
                        Calories = 350,
                        Image = $"{apiUrl}/images/salad2.jpg",
                        Category = categories.FirstOrDefault(c => c.NormalizedName == "salads")
                    },
                    new Dish
                    {
                        Name = "Куриный суп",
                        Description = "Домашний, с лапшой",
                        Calories = 280,
                        Image = $"{apiUrl}/images/soup2.jpg",
                        Category = categories.FirstOrDefault(c => c.NormalizedName == "soups")
                    },
                    new Dish
                    {
                        Name = "Лазанья",
                        Description = "Итальянская классика",
                        Calories = 650,
                        Image = $"{apiUrl}/images/main2.jpg",
                        Category = categories.FirstOrDefault(c => c.NormalizedName == "main")
                    }
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
