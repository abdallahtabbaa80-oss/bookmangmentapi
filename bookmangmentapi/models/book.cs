namespace bookmangmentapi.models
{
    public class Book
    {
        //model for book
        
        
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public int YearPublished { get; set; }
        public string isbn { get; set; }=string.Empty;
       public string? additionalDetails { get; set; }
        public int version { get; set;}
       

    }
}
