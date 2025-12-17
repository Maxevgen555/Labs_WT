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
    }
}
