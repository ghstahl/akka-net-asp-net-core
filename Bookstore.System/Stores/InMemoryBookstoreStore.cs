using Bookstore.Contracts;
using Bookstore.Domain;
using Bookstore.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Stores
{
    public class InMemoryBookstoreStore : IBookstoreStore
    {
        private readonly Dictionary<Guid, Book> _books = new Dictionary<Guid, Book>();
        public Task CreateBookAsync(Book newBook)
        {
            _books.Add(newBook.Id, newBook);
            return Task.CompletedTask;
        }

        public Task<BookDto> GetBookAsync(Guid id)
        {
            if (_books.TryGetValue(id, out var book))
                return Task.FromResult(GetBookDto(book));
            return Task.FromResult((BookDto)null);
        }

        public Task<IEnumerable<BookDto>> GetBooksAsync()
        {
            var d = _books.Select(x => GetBookDto(x.Value));
            return Task.FromResult(d);
        }
        private static BookDto GetBookDto(Book book) => new BookDto
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            Cost = book.Cost,
            InventoryAmount = book.InventoryAmount
        };
    }
}
