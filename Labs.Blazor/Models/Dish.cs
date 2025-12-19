namespace Labs.Blazor.Models
{
    public class Dish
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Calories { get; set; }
        public string? Image { get; set; }
        public int CategoryId { get; set; }
        public BlazorCategory? Category { get; set; }
    }

    public class BlazorCategory
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string NormalizedName { get; set; } = string.Empty;
    }
}
