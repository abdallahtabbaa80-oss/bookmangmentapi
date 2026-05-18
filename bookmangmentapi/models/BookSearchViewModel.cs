namespace bookmangmentapi.models
{
    public class BookSearchViewModel
    {
        public string? Isbn { get; set; }
        public int? Version { get; set; }
        public bool HasSearched { get; set; }
        public List<Book> Results { get; set; } = new();
        public string? ExternalDetails { get; set; }
        public string? ResultSource { get; set; }
    }
}
