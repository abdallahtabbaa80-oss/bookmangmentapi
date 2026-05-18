using bookmangmentapi.data;
using bookmangmentapi.models;
using bookmangmentapi.services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
namespace bookmangmentapi.Controllers

{//controller layer
    [Route("api/[controller]")]
    [ApiController]
    public class Bookcontroller : ControllerBase
    {
        private readonly Service _service;
        public Bookcontroller(Service service)
        {
            _service = service;
        }
        private static List<Book> books = Data.Books;



        // api methods 

        [HttpGet]
        public async Task<IActionResult> GetBooks()
        {
            foreach (var book in books)
            {
                
                if (string.IsNullOrEmpty(book.additionalDetails))
                    book.additionalDetails = await _service.GetBooksdescriptionAsync(book.isbn);
            }
            return Ok(books);
        }

        [HttpPost]

        public async Task<IActionResult> AddBook([FromBody] Book newBook)
        {
            if (newBook == null)
                return BadRequest("Book is null");


            newBook.additionalDetails = await _service.GetBooksdescriptionAsync(newBook.isbn);


            newBook.Id = books.Count == 0 ? 1 : books.Max(b => b.Id) + 1;

            Data.Books.Add(newBook);

            return CreatedAtAction(nameof(GetBookByisbn), new { id = newBook.Id }, newBook);
        }
       /* [HttpGet("{isbn}")]
        public async Task<IActionResult> GetBookById(string isbn)
        {
            var booksL = books.Where(b => b.isbn == isbn);
            if (booksL == null)
                return NotFound($"Book with id {isbn} not found.");
            foreach (var book in booksL)
            {
                if (string.IsNullOrEmpty(book.additionalDetails))
                    book.additionalDetails = await _service.GetBooksdescriptionAsync(book.isbn);
            }

            return Ok(booksL);
        }*/
        // 
        [HttpGet("{isbn}")]
        public async Task<IActionResult> GetBookByisbn(string isbn, [FromQuery] int? version=null)
        {
            List<Book> booksL;

            if (version == null)
            {
                booksL = books.Where(b => b.isbn == isbn).ToList();
            }
            else
            {
                booksL = books.Where(b => b.isbn == isbn && b.version == version.Value).ToList();
            }

            if (!booksL.Any())
                return NotFound($"Book with ISBN {isbn} not found.");

            foreach (var book in booksL)
            {
                if (string.IsNullOrEmpty(book.additionalDetails))
                    book.additionalDetails = await _service.GetBooksdescriptionAsync(book.isbn);
            }

            return Ok(booksL);
        }



        [HttpPut("{isbn}")]
        public IActionResult UpdateBook(string isbn, [FromBody] Book updatedBook)
        {
            if (updatedBook == null)
                return BadRequest("Invalid book data.");
            updatedBook.isbn = isbn;

            var existing = Data.GetByIsbn(isbn);
            if (existing == null)
                return NotFound($"Book with id {isbn} not found.");


            Data.Update(updatedBook);

            return NoContent();
        }

        [HttpDelete("{isbn}/{version}")]
        public IActionResult DeleteBook(string isbn,int version)
        {
            Data.Delete(isbn,version);

            return NoContent();
        }



    }

}