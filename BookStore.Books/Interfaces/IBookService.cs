using BookStore.Books.Models.Dtos;
using BookStore.Books.Models.Entities;
using BookStore.Books.Models;

namespace BookStore.Books.Interfaces
{
    public interface IBookService
    {
        Task<ApiResponse<Book>> AddBookAsync(CreateBookDto bookDto);
        Task<ApiResponse<Book>> UpdateBookAsync(int bookId, UpdateBookDto bookDto);
        Task<ApiResponse<Book>> DeleteBookAsync(int bookId);
        Task<ApiResponse<List<Book>>> GetAllBooksAsync();
    }
}
