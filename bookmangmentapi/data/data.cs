using bookmangmentapi.models;

namespace bookmangmentapi.data
{
    public static class Data
    {
        public static List<Book> Books { get; } = new List<Book>
        {
            new Book { Id = 1, Title = "book 1", Author = "Author A", YearPublished = 2020, isbn = "ISBN002", additionalDetails = "no additional details", version = 4 },
            new Book { Id = 2, Title = "book 1", Author = "Author A", YearPublished = 2020, isbn = "ISBN002", additionalDetails = "no additional details", version = 3 },
            new Book { Id = 3, Title = "Clean code", Author = "Robert C. Martin", YearPublished = 2009, isbn = "9780132350884", version = 1 }
        };

        // Get all versions of a book by ISBN
        public static List<Book>? GetByIsbn(string isbn)
        {
            return Books.Where(b => b.isbn == isbn).ToList();
        }

        // Get specific version
        public static List<Book> GetByIsbn(string isbn, int? version = null)
        {
            
            if (version == null)
                return Books.Where(b => b.isbn == isbn).ToList();

           
            return Books.Where(b => b.isbn == isbn && b.version == version).ToList();
        }


        // Add new book
        public static void Add(Book newBook)
        {
            newBook.Id = Books.Count + 1;
            Books.Add(newBook);
        }

        // Update a book by ISBN + version
        public static bool Update(Book updatedBook)
        {
            var existingBook = GetByIsbn(updatedBook.isbn, updatedBook.version).FirstOrDefault();

            if (existingBook == null)
                return false;

            existingBook.Title = updatedBook.Title;
            existingBook.Author = updatedBook.Author;
            existingBook.YearPublished = updatedBook.YearPublished;
            existingBook.additionalDetails = updatedBook.additionalDetails;

            return true;
        }

        // Delete by ISBN + version
        public static bool Delete(string isbn, int version)
        {
            var book = GetByIsbn(isbn, version).FirstOrDefault();
            if (book == null) return false;

            Books.Remove(book);

            return true;
        }
    }
}
