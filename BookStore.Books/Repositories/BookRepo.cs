using BookStore.Books.Data;
using BookStore.Books.Interfaces;
using BookStore.Books.Models;
using BookStore.Books.Models.Dtos;
using BookStore.Books.Models.Entities;
using Microsoft.EntityFrameworkCore;
using static Azure.Core.HttpHeader;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace BookStore.Books.Repositories
{
    public class BookRepo : IBookRepo
    {
        private readonly BookDbContext _context;
        private readonly IDistributedCache _distributedCache;
        public BookRepo(BookDbContext context, IDistributedCache distributedCache)
        {
            _context = context;
            _distributedCache = distributedCache;
        }

        public async Task<ApiResponse<Book>> AddBookToDbAsync(CreateBookDto bookDto)
        {
            var book = new Book
            {
                Title = bookDto.Title,
                Author = bookDto.Author,
                Description = bookDto.Description,
                StockQuantity = bookDto.StockQuantity,
                Price = bookDto.Price,
                ImageUrl = bookDto.ImageUrl
            };
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            var response = new ApiResponse<Book>
            {
                Success = true,
                Message = "Book created successfully",
                Data = book
            };

            return response;
        }
        public async Task<ApiResponse<Book>> UpdateBookInDbAsync(int bookId, UpdateBookDto bookDto)
        {
            var book = await _context.Books.FirstOrDefaultAsync(b => b.BookId == bookId);
            if (book == null)
            {
                return new ApiResponse<Book>
                {
                    Success = false,
                    Message = "Book not found"
                };
            }

            book.Title = bookDto.Title;
            book.Author = bookDto.Author;
            book.Description = bookDto.Description;
            book.StockQuantity = bookDto.StockQuantity;
            book.Price = bookDto.Price;
            book.ImageUrl = bookDto.ImageUrl;

            _context.Books.Update(book);
            await _context.SaveChangesAsync();

            return new ApiResponse<Book>
            {
                Success = true,
                Message = "Book updated successfully",
                Data = book
            };
        }

        public async Task<ApiResponse<Book>> DeleteBookFromDbAsync(int bookId)
        {
            var book = await _context.Books.FindAsync(bookId);
            if (book == null)
            {
                return new ApiResponse<Book>
                {
                    Success = false,
                    Message = "Book not found"
                };
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return new ApiResponse<Book>
            {
                Success = true,
                Message = "Book deleted successfully"
            };
        }

        public async Task<ApiResponse<List<Book>>> GetAllBooksFromDbAsync()
        {
            var books = await _context.Books.ToListAsync();
            var serializedbooks = JsonSerializer.Serialize(books);
            await _distributedCache.SetStringAsync($"allBooks", serializedbooks, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });

            return new ApiResponse<List<Book>>
            {
                Success = true,
                Message = "Books retrieved successfully",
                Data = books
            };
        }

        public async Task<ApiResponse<Book>> GetBookByIdFromDb(int Id)
        {
            var book = await _context.Books.FindAsync(Id);
            if (book == null)
            {
                return new ApiResponse<Book>
                {
                    Success = false,
                    Message = "Book Not found"
                };
            }
            else
            {
                return new ApiResponse<Book>
                {
                    Success = true,
                    Message = "Book retrieved successfully",
                    Data = book
                };


            }

        }
    }
}