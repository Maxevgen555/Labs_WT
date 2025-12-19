using Labs.Blazor.Models;
using System.Net.Http.Json;

namespace Labs.Blazor.Services
{
    public class ApiProductService : IProductService<Dish>
    {
        private readonly HttpClient _http;
        private List<Dish> _dishes = new();
        private int _currentPage = 1;
        private int _totalPages = 1;

        public ApiProductService(HttpClient http)
        {
            _http = http;
        }

        public IEnumerable<Dish> Products => _dishes;
        public int CurrentPage => _currentPage;
        public int TotalPages => _totalPages;
        public event Action? ListChanged;

        public async Task GetProducts(int pageNo = 1, int pageSize = 3)
        {
            try
            {
                var queryString = $"?pageNo={pageNo}&pageSize={pageSize}";
                var response = await _http.GetAsync(queryString);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content
                        .ReadFromJsonAsync<ResponseData<ListModel<Dish>>>();

                    if (result?.Data != null)
                    {
                        _dishes = result.Data.Items ?? new();
                        _currentPage = result.Data.CurrentPage;
                        _totalPages = result.Data.TotalPages;
                        ListChanged?.Invoke();
                        return;
                    }
                }

                _dishes = new();
                _currentPage = 1;
                _totalPages = 1;
                ListChanged?.Invoke();
            }
            catch (Exception)
            {
                _dishes = new();
                _currentPage = 1;
                _totalPages = 1;
                ListChanged?.Invoke();
            }
        }
    }
}