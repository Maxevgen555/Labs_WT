using Labs.API.Data;
using Labs.Domain.Entities;
using Labs.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Labs.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DishesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public DishesController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: api/Dishes
        [HttpGet]
        public async Task<ActionResult<ResponseData<ListModel<Dish>>>> GetDishes(
            string? category,
            int pageNo = 1,
            int pageSize = 3)
        {
            // Создать объект результата
            var result = new ResponseData<ListModel<Dish>>();

            // Фильтрация по категории с загрузкой данных категории
            IQueryable<Dish> query = _context.Dishes.Include(d => d.Category);

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(d => d.Category != null &&
                           d.Category.NormalizedName.Equals(category));
            }

            // Подсчет общего количества записей
            var totalCount = await query.CountAsync();

            // Вычисление общего количества страниц
            int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            // Корректировка номера страницы
            if (pageNo < 1) pageNo = 1;
            if (pageNo > totalPages && totalPages > 0) pageNo = totalPages;

            // Получение данных для текущей страницы
            var items = await query
                .Skip((pageNo - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Создание объекта ListModel с нужной страницей данных
            var listData = new ListModel<Dish>()
            {
                Items = items,
                CurrentPage = pageNo,
                TotalPages = totalPages
            };

            // Поместить данные в объект результата
            result.Data = listData;

            // Если список пустой
            if (totalCount == 0)
            {
                result.Success = false;
                result.ErrorMessage = "Нет объектов в выбранной категории";
            }

            return Ok(result);
        }

        // GET: api/Dishes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseData<Dish>>> GetDish(int id)
        {
            var dish = await _context.Dishes
                .Include(d => d.Category)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (dish == null)
            {
                return NotFound(new ResponseData<Dish>
                {
                    Success = false,
                    ErrorMessage = "Блюдо не найдено"
                });
            }

            return Ok(new ResponseData<Dish> { Data = dish });
        }

        // PUT: api/Dishes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDish(int id, Dish dish)
        {
            if (id != dish.Id)
            {
                return BadRequest();
            }

            _context.Entry(dish).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DishExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Dishes
        [HttpPost]
        public async Task<ActionResult<ResponseData<Dish>>> PostDish(Dish dish)
        {
            _context.Dishes.Add(dish);
            await _context.SaveChangesAsync();

            var response = new ResponseData<Dish>
            {
                Data = dish
            };

            return CreatedAtAction("GetDish", new { id = dish.Id }, response);
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> SaveImage(int id, IFormFile image)
        {
            // Найти объект по Id
            var dish = await _context.Dishes.FindAsync(id);
            if (dish == null)
            {
                return NotFound();
            }

            // Путь к папке wwwroot/images
            var imagesPath = Path.Combine(_env.WebRootPath, "images");

            // Создать папку, если её нет
            if (!Directory.Exists(imagesPath))
            {
                Directory.CreateDirectory(imagesPath);
            }

            // Получить случайное имя файла
            var randomName = Path.GetRandomFileName();
            // Получить расширение исходного файла
            var extension = Path.GetExtension(image.FileName);
            // Задать в новом имени расширение как в исходном файле
            var fileName = Path.ChangeExtension(randomName, extension);
            // Полный путь к файлу
            var filePath = Path.Combine(imagesPath, fileName);

            // Сохранить файл
            using var stream = System.IO.File.Create(filePath);
            await image.CopyToAsync(stream);

            // Получить URL файла изображения
            var host = $"{Request.Scheme}://{Request.Host}";
            var url = $"{host}/images/{fileName}";

            // Сохранить URL файла в объекте
            dish.Image = url;
            await _context.SaveChangesAsync();

            return Ok();
        }
        // DELETE: api/Dishes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDish(int id)
        {
            var dish = await _context.Dishes.FindAsync(id);
            if (dish == null)
            {
                return NotFound();
            }

            // Удалить файл изображения, если он существует
            if (!string.IsNullOrEmpty(dish.Image))
            {
                try
                {
                    var fileName = Path.GetFileName(dish.Image);
                    var imagesPath = Path.Combine(_env.WebRootPath, "images", fileName);
                    if (System.IO.File.Exists(imagesPath))
                    {
                        System.IO.File.Delete(imagesPath);
                    }
                }
                catch (Exception ex)
                {
                    // Логировать ошибку, но не прерывать удаление
                    Console.WriteLine($"Ошибка при удалении файла: {ex.Message}");
                }
            }

            _context.Dishes.Remove(dish);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        private bool DishExists(int id)
        {
            return _context.Dishes.Any(e => e.Id == id);
        }
    }
}
