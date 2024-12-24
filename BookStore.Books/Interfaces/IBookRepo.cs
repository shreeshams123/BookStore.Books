using BookStore.Books.Models.Dtos;
using BookStore.Books.Models.Entities;
using BookStore.Books.Models;
using System.Threading.Tasks;

namespace BookStore.Books.Interfaces
{
    public interface IBookRepo
    {
        Task<ApiResponse<Book>> AddBookToDbAsync(CreateBookDto bookDto);
        Task<ApiResponse<Book>> UpdateBookInDbAsync(int bookId, UpdateBookDto bookDto);
        Task<ApiResponse<Book>> DeleteBookFromDbAsync(int bookId);
        Task<ApiResponse<List<Book>>> GetAllBooksFromDbAsync();
    }
}
