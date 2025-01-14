﻿using Akka.Actor;
using Bookstore.Contracts;
using Bookstore.Dto;
using Bookstore.Messages;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bookstore.Domain
{
    public class BooksManagerActor : ReceiveActor
    {
       
        private IBookstoreStore _bookstoreStore;
        private Guid LatestGuid { get; set; }
        public BooksManagerActor(IBookstoreStore bookstoreStore)
        {
            _bookstoreStore = bookstoreStore;
            ReceiveAsync<CreateBook>(async command =>
            {
                LatestGuid = Guid.NewGuid();
                var newBook = new Book
                {
                    Id = LatestGuid,
                    Title = command.Title,
                    Author = command.Author,
                    Cost = command.Cost,
                    InventoryAmount = command.InventoryAmount,
                };

                await _bookstoreStore.CreateBookAsync(newBook);
            });

            
            ReceiveAsync<GetLatestGuid>(async _ =>  
            {
                 Sender.Tell(LatestGuid);
            });

            ReceiveAsync<GetBookById>(async query =>  
            {
                var bookDto = await _bookstoreStore.GetBookAsync(query.Id);
                Sender.Tell(bookDto);
            });

            ReceiveAsync<GetBooks>(async query => {
                var books = await _bookstoreStore.GetBooksAsync();
                Sender.Tell(books);
            });

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