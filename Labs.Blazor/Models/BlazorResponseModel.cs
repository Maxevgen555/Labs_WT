namespace Labs.Blazor.Models
{
    public class BlazorResponseModel<T>
    {
        public T? Data { get; set; }
        public bool Success { get; set; } = true;
        public string? ErrorMessage { get; set; }
    }
}
