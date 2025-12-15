using Labs.Domain.Entities;
using Labs.Domain.Models;

namespace Labs.UI.Services.Contracts
{
    public interface ICategoryService
    {
        Task<ResponseData<List<Category>>> GetCategoryListAsync();
    }
}