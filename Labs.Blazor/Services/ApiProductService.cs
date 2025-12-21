using Labs.Domain.Entities;
using Labs.Domain.Models;
using Labs.Blazor.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace Labs.Blazor.Services
{
    public class ApiProductService : IProductService<BlazorDishModel>
    {
        private readonly HttpClient _http;
        private readonly ILogger<ApiProductService> _logger;
        private List<BlazorDishModel> _dishes = new();
        private int _currentPage = 1;
        private int _totalPages = 1;

        public ApiProductService(HttpClient http, ILogger<ApiProductService> logger)
        {
            _http = http;
            _logger = logger;
        }

        public IEnumerable<BlazorDishModel> Products => _dishes;
        public int CurrentPage => _currentPage;
        public int TotalPages => _totalPages;
        public event Action? ListChanged;

        public async Task GetProducts(int pageNo = 1, int pageSize = 3)
        {
            _logger.LogDebug("=== НАЧАЛО GetProducts ===");

            try
            {
                // Правильное формирование URL
                var url = $"api/dishes?pageNo={pageNo}&pageSize={pageSize}";
                _logger.LogInformation($"Делаем запрос к: {url}");

                var response = await _http.GetAsync(url);
                _logger.LogInformation($"Статус ответа: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    _logger.LogDebug($"Получен JSON (первые 500 символов): {jsonString.Substring(0, Math.Min(500, jsonString.Length))}");

                    try
                    {
                        var result = JsonSerializer.Deserialize<ResponseData<ListModel<Labs.Domain.Entities.Dish>>>(
                            jsonString,
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                        if (result?.Data?.Items != null)
                        {
                            _dishes = result.Data.Items.Select(d => new BlazorDishModel
                            {
                                Id = d.Id,
                                Name = d.Name,
                                Description = d.Description,
                                Calories = d.Calories,
                                Image = d.Image,
                                CategoryId = d.CategoryId,
                                Category = d.Category != null ? new BlazorCategoryModel
                                {
                                    Id = d.Category.Id,
                                    Name = d.Category.Name,
                                    NormalizedName = d.Category.NormalizedName
                                } : null
                            }).ToList();

                            _currentPage = result.Data.CurrentPage;
                            _totalPages = result.Data.TotalPages;

                            _logger.LogInformation($"Успешно загружено {_dishes.Count} блюд");
                        }
                        else
                        {
                            _logger.LogWarning("Результат десериализации пуст");
                        }
                    }
                    catch (JsonException jsonEx)
                    {
                        _logger.LogError(jsonEx, $"Ошибка десериализации JSON: {jsonEx.Message}");
                        if (!string.IsNullOrEmpty(jsonString))
                        {
                            _logger.LogDebug($"JSON который не десериализовался (первые 200 символов): {jsonString.Substring(0, Math.Min(200, jsonString.Length))}");
                        }
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Ошибка HTTP: {response.StatusCode}. Контент: {errorContent}");
                }
            }
            catch (HttpRequestException httpEx)
            {
                _logger.LogError(httpEx, $"HTTP исключение: {httpEx.Message}");
                if (httpEx.InnerException != null)
                {
                    _logger.LogError($"Внутреннее исключение: {httpEx.InnerException.Message}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Критическая ошибка: {ex.Message}");
            }

            _logger.LogDebug($"Количество блюд после загрузки: {_dishes.Count}");
            _logger.LogDebug("=== КОНЕЦ GetProducts ===");

            // Уведомляем об изменении
            ListChanged?.Invoke();
        }
    }
}