using Labs.Domain.Entities;
using Labs.Domain.Models;
using Labs.UI.Services.Contracts;
using System.Net.Http.Json;

namespace Labs.UI.Services
{
    public class ApiCategoryService : ICategoryService
    {
        private readonly HttpClient _httpClient;

        public ApiCategoryService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ResponseData<List<Category>>> GetCategoryListAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content
                        .ReadFromJsonAsync<ResponseData<List<Category>>>();

                    if (result != null)
                    {
                        return result;
                    }
                }

                return new ResponseData<List<Category>>
                {
                    Success = false,
                    ErrorMessage = "Ошибка при получении данных с API"
                };
            }
            catch (Exception ex)
            {
                return new ResponseData<List<Category>>
                {
                    Success = false,
                    ErrorMessage = $"Ошибка: {ex.Message}"
                };
            }
        }
    }
}
