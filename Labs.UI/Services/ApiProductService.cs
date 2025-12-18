using Labs.Domain.Entities;
using Labs.Domain.Models;
using Labs.UI.Services.Contracts;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Json;

namespace Labs.UI.Services
{
    public class ApiProductService : IProductService
    {
        private readonly HttpClient _httpClient;

        public ApiProductService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ResponseData<ListModel<Dish>>> GetProductListAsync(
            string? categoryNormalizedName,
            int pageNo = 1,
            int pageSize = 3)  // Добавляем параметр
        {
            try
            {
                var queryString = $"?pageNo={pageNo}&pageSize={pageSize}";  // Используем pageSize

                if (!string.IsNullOrEmpty(categoryNormalizedName))
                {
                    queryString += $"&category={categoryNormalizedName}";
                }

                var response = await _httpClient.GetAsync(queryString);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content
                        .ReadFromJsonAsync<ResponseData<ListModel<Dish>>>();

                    if (result != null)
                    {
                        return result;
                    }
                }

                return new ResponseData<ListModel<Dish>>
                {
                    Success = false,
                    ErrorMessage = "Ошибка при получении данных с API"
                };
            }
            catch (Exception ex)
            {
                return new ResponseData<ListModel<Dish>>
                {
                    Success = false,
                    ErrorMessage = $"Ошибка: {ex.Message}"
                };
            }
        }
        public async Task<ResponseData<Dish>> GetProductByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{id}");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content
                        .ReadFromJsonAsync<ResponseData<Dish>>();

                    if (result != null)
                    {
                        return result;
                    }
                }

                return new ResponseData<Dish>
                {
                    Success = false,
                    ErrorMessage = "Ошибка при получении данных с API"
                };
            }
            catch (Exception ex)
            {
                return new ResponseData<Dish>
                {
                    Success = false,
                    ErrorMessage = $"Ошибка: {ex.Message}"
                };
            }
        }
        public async Task<ResponseData<Dish>> CreateProductAsync(Dish product, IFormFile? formFile)
        {
            try
            {
                // В реальном приложении нужно отправвать multipart/form-data с файлом
                // Пока отправляем только JSON
                var response = await _httpClient.PostAsJsonAsync("", product);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ResponseData<Dish>>();
                    if (result != null)
                    {
                        return result;
                    }
                }

                return new ResponseData<Dish>
                {
                    Success = false,
                    ErrorMessage = $"Ошибка при создании: {response.StatusCode}"
                };
            }
            catch (Exception ex)
            {
                return new ResponseData<Dish>
                {
                    Success = false,
                    ErrorMessage = $"Ошибка: {ex.Message}"
                };
            }
        }

        public async Task UpdateProductAsync(int id, Dish product, IFormFile? formFile)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{id}", product);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Ошибка при обновлении: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при обновлении: {ex.Message}");
            }
        }

        public async Task DeleteProductAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{id}");
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Ошибка при удалении: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при удалении: {ex.Message}");
            }
        }
    }
}
