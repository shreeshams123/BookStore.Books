using BookStore.Books.Models.Dtos;
using BookStore.Books.Models.Entities;
using BookStore.Books.Models;
using System.Threading.Tasks;

namespace BookStore.Books.Interfaces
{
    public interface IBookRepo
    {
        Task<ApiResponse<Book>> AddBookToDbAsync(CreateBookDto bookDto, byte[] imageBytes);
        Task<ApiResponse<Book>> UpdateBookInDbAsync(int bookId, UpdateBookDto bookDto, byte[] imageBytes = null);
        Task<ApiResponse<Book>> DeleteBookFromDbAsync(int bookId);
        Task<ApiResponse<List<BookResponseDto>>> GetAllBooksFromDbAsync();
        Task<ApiResponse<BookResponseDto>> GetBookByIdFromDb(int Id);
    }
}
