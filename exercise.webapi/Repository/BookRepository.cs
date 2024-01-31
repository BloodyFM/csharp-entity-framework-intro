﻿using exercise.webapi.Data;
using exercise.webapi.Models;
using Microsoft.EntityFrameworkCore;

namespace exercise.webapi.Repository
{
    public class BookRepository: IBookRepository
    {
        DatabaseContext _db;
        
        public BookRepository(DatabaseContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<BookDTO>> GetAllBooks()
        {
            var books = await _db.Books.Include(b => b.Author).ToListAsync();
            var returnList = new List<BookDTO>();
            foreach (var book in books)
            {
                returnList.Add(bookToDTO(book));
            }
            return returnList;
        }

        public async Task<BookDTO?> GetBookById(int id)
        {
            var book = await _db.Books.Include(b => b.Author).FirstOrDefaultAsync(x => x.Id == id);
            if (book == null)
                return null;

            return bookToDTO(book);
        }

        public async Task<BookDTO?> UpdateBook(int id, BookPush newBook)
        {
            var bookToUpdate = await _db.Books.Include(b => b.Author).FirstOrDefaultAsync(x => x.Id == id);
            if (bookToUpdate == null) return null;
            var author = await _db.Authors.FirstOrDefaultAsync(x => x.Id == newBook.AuthorId);
            if (author == null) return null;
            newBook.Title = !string.IsNullOrEmpty(newBook.Title) ? bookToUpdate.Title = newBook.Title : null!;
            bookToUpdate.Author = author;
            await _db.SaveChangesAsync();
            return bookToDTO(bookToUpdate);
        }

        private BookDTO bookToDTO(Book book)
        {
            return new BookDTO()
                {
                    Title = book.Title,
                    Author = new AuthorDTO()
                    {
                        FirstName = book.Author.FirstName,
                        LastName = book.Author.LastName,
                        Email = book.Author.Email
                    }
                };
        }
    }
}
