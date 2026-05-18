using bookmangmentapi.data;
using bookmangmentapi.models;
using bookmangmentapi.services;
using Microsoft.AspNetCore.Mvc;

namespace bookmangmentapi.Controllers
{
    public class BooksViewController : Controller
    {
        private readonly Service _service;

        public BooksViewController(Service service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            await EnsureAdditionalDetailsAsync(Data.Books);
            return View(Data.Books);
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            await EnsureAdditionalDetailsAsync(Data.Books);
            return View(Data.Books);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new Book { version = 1 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Book newBook)
        {
            if (!ModelState.IsValid)
            {
                return View(newBook);
            }

            newBook.additionalDetails = await _service.GetBooksdescriptionAsync(newBook.isbn);
            newBook.Id = Data.Books.Count == 0 ? 1 : Data.Books.Max(b => b.Id) + 1;

            Data.Books.Add(newBook);
            TempData["Message"] = "Book added successfully.";

            return RedirectToAction(nameof(List));
        }

        [HttpGet]
        public async Task<IActionResult> Search(string? isbn, int? version)
        {
            var model = new BookSearchViewModel
            {
                Isbn = isbn,
                Version = version,
                HasSearched = !string.IsNullOrWhiteSpace(isbn)
            };

            if (model.HasSearched)
            {
                model.Results = Data.GetByIsbn(isbn!, version);

                if (model.Results.Any())
                {
                    model.ResultSource = "Local";
                }
                else
                {
                    model.ResultSource = "Google Books API";
                    model.ExternalDetails = await _service.GetBooksdescriptionAsync(isbn!);
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string isbn, int version)
        {
            var book = Data.GetByIsbn(isbn, version).FirstOrDefault();

            if (book == null)
            {
                TempData["Error"] = $"Book with ISBN {isbn} and version {version} was not found.";
                return RedirectToAction(nameof(List));
            }

            await EnsureAdditionalDetailsAsync(new[] { book });

            return View(CopyBook(book));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Book updatedBook)
        {
            if (!ModelState.IsValid)
            {
                return View(updatedBook);
            }

            if (!Data.Update(updatedBook))
            {
                ModelState.AddModelError(string.Empty, "The selected book could not be found.");
                return View(updatedBook);
            }

            TempData["Message"] = "Book updated successfully.";
            return RedirectToAction(nameof(List));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string isbn, int version)
        {
            var book = Data.GetByIsbn(isbn, version).FirstOrDefault();

            if (book == null)
            {
                TempData["Error"] = $"Book with ISBN {isbn} and version {version} was not found.";
                return RedirectToAction(nameof(List));
            }

            await EnsureAdditionalDetailsAsync(new[] { book });

            return View(CopyBook(book));
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string isbn, int version)
        {
            Data.Delete(isbn, version);
            TempData["Message"] = "Book deleted successfully.";

            return RedirectToAction(nameof(List));
        }

        private async Task EnsureAdditionalDetailsAsync(IEnumerable<Book> books)
        {
            foreach (var book in books)
            {
                if (string.IsNullOrEmpty(book.additionalDetails))
                {
                    book.additionalDetails = await _service.GetBooksdescriptionAsync(book.isbn);
                }
            }
        }

        private static Book CopyBook(Book book)
        {
            return new Book
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                YearPublished = book.YearPublished,
                isbn = book.isbn,
                additionalDetails = book.additionalDetails,
                version = book.version
            };
        }
    }
}
