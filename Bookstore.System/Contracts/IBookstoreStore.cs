using Akka.Actor;
using Bookstore.Domain;
using Bookstore.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Contracts
{
    public interface IBookstoreStore
    {
        Task CreateBookAsync(Book book);
        Task<IEnumerable<BookDto>> GetBooksAsync();
        Task<BookDto> GetBookAsync(Guid id);
    }
}
