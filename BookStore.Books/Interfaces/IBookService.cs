using BookStore.Books.Models.Dtos;
using BookStore.Books.Models.Entities;
using BookStore.Books.Models;

namespace BookStore.Books.Interfaces
{
    public interface IBookService
    {
        Task<ApiResponse<Book>> AddBookAsync(CreateBookDto bookDto, byte[] imageBytes);
        Task<ApiResponse<Book>> UpdateBookAsync(int bookId, UpdateBookDto bookDto, byte[] imageBytes = null);
        Task<ApiResponse<Book>> DeleteBookAsync(int bookId);
        Task<ApiResponse<List<BookResponseDto>>> GetAllBooksAsync();
        Task<ApiResponse<BookResponseDto>> GetBookByIdAsync(int Id);
    }
}
