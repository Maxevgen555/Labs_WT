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

                // Используем хост приложения для формирования URL
                var host = "https://localhost:7002"; // или "http://localhost:5002" если используете HTTP

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

                // Создаем блюда с полными URL к изображениям
                var dishes = new List<Dish>
                {
                    new Dish
                    {
                        Name = "Суп-харчо",
                        Description = "Очень острый, невкусный",
                        Calories = 200,
                        Image = $"{host}/images/soup.jpg",
                        CategoryId = categories.First(c => c.NormalizedName == "soups").Id
                    },
                    new Dish
                    {
                        Name = "Борщ",
                        Description = "Много сала, без сметаны",
                        Calories = 330,
                        Image = $"{host}/images/borshch.jpg",
                        CategoryId = categories.First(c => c.NormalizedName == "soups").Id
                    },
                    new Dish
                    {
                        Name = "Цезарь",
                        Description = "С курицей и пармезаном",
                        Calories = 450,
                        Image = $"{host}/images/salad.jpg",
                        CategoryId = categories.First(c => c.NormalizedName == "salads").Id
                    },
                    new Dish
                    {
                        Name = "Стейк",
                        Description = "Мраморная говядина",
                        Calories = 800,
                        Image = $"{host}/images/steak.jpg",
                        CategoryId = categories.First(c => c.NormalizedName == "main").Id
                    },
                    new Dish
                    {
                        Name = "Греческий салат",
                        Description = "С фетой и оливками",
                        Calories = 350,
                        Image = $"{host}/images/salad2.jpg",
                        CategoryId = categories.First(c => c.NormalizedName == "salads").Id
                    },
                    new Dish
                    {
                        Name = "Куриный суп",
                        Description = "Домашний, с лапшой",
                        Calories = 280,
                        Image = $"{host}/images/soup2.jpg",
                        CategoryId = categories.First(c => c.NormalizedName == "soups").Id
                    },
                    new Dish
                    {
                        Name = "Лазанья",
                        Description = "Итальянская классика",
                        Calories = 650,
                        Image = $"{host}/images/main2.jpg",
                        CategoryId = categories.First(c => c.NormalizedName == "main").Id
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