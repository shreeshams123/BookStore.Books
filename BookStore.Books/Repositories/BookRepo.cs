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

        public async Task<ApiResponse<Book>> AddBookToDbAsync(CreateBookDto bookDto, byte[] imageBytes)
        {
            var book = new Book
            {
                Title = bookDto.Title,
                Author = bookDto.Author,
                Description = bookDto.Description,
                StockQuantity = bookDto.StockQuantity,
                Price = bookDto.Price,
                Image = imageBytes  
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

        public async Task<ApiResponse<Book>> UpdateBookInDbAsync(int bookId, UpdateBookDto bookDto, byte[] imageBytes = null)
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

            book.Title = string.IsNullOrEmpty(bookDto.Title) ? book.Title : bookDto.Title;
            book.Author = string.IsNullOrEmpty(bookDto.Author) ? book.Author : bookDto.Author;
            book.Description = string.IsNullOrEmpty(bookDto.Description) ? book.Description : bookDto.Description;
            book.StockQuantity = bookDto.StockQuantity ?? book.StockQuantity;
            book.Price = bookDto.Price ?? book.Price;

            if (imageBytes != null && imageBytes.Length > 0)
            {
                book.Image = imageBytes;
            }

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

        public async Task<ApiResponse<List<BookResponseDto>>> GetAllBooksFromDbAsync()
        {
            var books = await _context.Books.ToListAsync();

            var bookDtos = books.Select(book => new BookResponseDto
            {
                BookId = book.BookId,
                Title = book.Title,
                Author = book.Author,
                Description = book.Description,
                StockQuantity = book.StockQuantity,
                Price = book.Price,
                Image = book.Image != null && book.Image.Length > 0 ? Convert.ToBase64String(book.Image) : null
            }).ToList();

            var serializedBooks = JsonSerializer.Serialize(bookDtos);
            await _distributedCache.SetStringAsync($"allBooks", serializedBooks, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });

            return new ApiResponse<List<BookResponseDto>>
            {
                Success = true,
                Message = "Books retrieved successfully",
                Data = bookDtos
            };
        }


        public async Task<ApiResponse<BookResponseDto>> GetBookByIdFromDb(int Id)
        {
            var book = await _context.Books.FindAsync(Id);
            if (book == null)
            {
                return new ApiResponse<BookResponseDto>
                {
                    Success = false,
                    Message = "Book Not found"
                };
            }
            else
            {
                string imageBase64 = book.Image != null ? Convert.ToBase64String(book.Image) : null;
                return new ApiResponse<BookResponseDto>
                {
                    Success = true,
                    Message = "Book retrieved successfully",
                    Data = new BookResponseDto
                    {
                        BookId = book.BookId,
                        Title = book.Title,
                        Author = book.Author,
                        Description = book.Description,
                        StockQuantity = book.StockQuantity,
                        Price = book.Price,
                        Image = imageBase64
                    }
                };


            }

        }
    }
}