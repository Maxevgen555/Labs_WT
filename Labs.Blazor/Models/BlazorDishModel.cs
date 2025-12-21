namespace Labs.Blazor.Models
{
    public class BlazorDishModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Calories { get; set; }
        public string? Image { get; set; }
        public int CategoryId { get; set; }
        public BlazorCategoryModel? Category { get; set; }
    }

    public class BlazorCategoryModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string NormalizedName { get; set; } = string.Empty;
    }
}
