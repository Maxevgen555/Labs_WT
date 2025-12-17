using Labs.Domain.Entities;
using Labs.Domain.Models;
using Labs.UI.Services.Contracts;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Json;
using System.Text.Json;

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
            int pageNo = 1)
        {
            try
            {
                var queryString = $"?pageNo={pageNo}&pageSize=3";

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
                    ErrorMessage = $"Блюдо с ID {id} не найдено"
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
            var responseData = new ResponseData<Dish>();
            var serializerOptions = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            try
            {
                // Послать запрос к API для сохранения объекта
                var response = await _httpClient.PostAsJsonAsync("", product, serializerOptions);

                if (!response.IsSuccessStatusCode)
                {
                    responseData.Success = false;
                    responseData.ErrorMessage = $"Не удалось создать объект: {response.StatusCode}";
                    return responseData;
                }

                // Если файл изображения передан клиентом
                if (formFile != null)
                {
                    // получить созданный объект из ответа Api-сервиса
                    var dish = await response.Content.ReadFromJsonAsync<Dish>();
                    if (dish == null)
                    {
                        responseData.Success = false;
                        responseData.ErrorMessage = "Не удалось получить созданный объект";
                        return responseData;
                    }

                    // создать объект запроса
                    var request = new HttpRequestMessage
                    {
                        Method = HttpMethod.Post,
                        RequestUri = new Uri($"{_httpClient.BaseAddress}{dish.Id}")
                    };

                    // Создать контент типа multipart form-data
                    var content = new MultipartFormDataContent();
                    // создать потоковый контент из переданного файла
                    var streamContent = new StreamContent(formFile.OpenReadStream());
                    // добавить потоковый контент в общий контент по именем "image"
                    content.Add(streamContent, "image", formFile.FileName);
                    // поместить контент в запрос
                    request.Content = content;
                    // послать запрос к Api-сервису
                    response = await _httpClient.SendAsync(request);

                    if (!response.IsSuccessStatusCode)
                    {
                        responseData.Success = false;
                        responseData.ErrorMessage = $"Не удалось сохранить изображение: {response.StatusCode}";
                    }
                    else
                    {
                        // Обновить объект, чтобы получить актуальное изображение
                        var updatedResponse = await _httpClient.GetAsync($"{dish.Id}");
                        if (updatedResponse.IsSuccessStatusCode)
                        {
                            var updatedDish = await updatedResponse.Content.ReadFromJsonAsync<ResponseData<Dish>>();
                            if (updatedDish != null && updatedDish.Success)
                            {
                                responseData.Data = updatedDish.Data;
                            }
                        }
                    }
                }
                else
                {
                    // Если файла нет, то просто вернуть результат создания
                    var dish = await response.Content.ReadFromJsonAsync<Dish>();
                    responseData.Data = dish;
                }

                return responseData;
            }
            catch (Exception ex)
            {
                responseData.Success = false;
                responseData.ErrorMessage = $"Ошибка при создании: {ex.Message}";
                return responseData;
            }
        }

        public async Task UpdateProductAsync(int id, Dish product, IFormFile? formFile)
        {
            try
            {
                // Сначала обновляем данные продукта
                var response = await _httpClient.PutAsJsonAsync($"{id}", product);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Ошибка при обновлении: {response.StatusCode}");
                }

                // Если передан файл изображения, то обновляем изображение
                if (formFile != null)
                {
                    var request = new HttpRequestMessage
                    {
                        Method = HttpMethod.Post,
                        RequestUri = new Uri($"{_httpClient.BaseAddress}{id}")
                    };

                    var content = new MultipartFormDataContent();
                    var streamContent = new StreamContent(formFile.OpenReadStream());
                    content.Add(streamContent, "image", formFile.FileName);
                    request.Content = content;

                    response = await _httpClient.SendAsync(request);
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"Ошибка при обновлении изображения: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка обновления продукта: {ex.Message}", ex);
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
                throw new Exception($"Ошибка удаления продукта: {ex.Message}", ex);
            }
        }
    }
}