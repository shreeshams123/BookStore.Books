using BookStore.Books.Models.Dtos;
using BookStore.Books.Models.Entities;
using BookStore.Books.Models;
using BookStore.Books.Interfaces;

namespace BookStore.Books.Services
{
    public class BookService:IBookService
    {
        private readonly IBookRepo _bookRepo;
        public BookService(IBookRepo bookRepo)
        {
            _bookRepo = bookRepo;
        }
        public async Task<ApiResponse<Book>> AddBookAsync(CreateBookDto bookDto, byte[] imageBytes)
        {
            return await _bookRepo.AddBookToDbAsync(bookDto, imageBytes);
        }

        public async Task<ApiResponse<Book>> UpdateBookAsync(int bookId, UpdateBookDto bookDto, byte[] imageBytes = null)
        {
            return await _bookRepo.UpdateBookInDbAsync(bookId, bookDto, imageBytes);
        }


        public async Task<ApiResponse<Book>> DeleteBookAsync(int bookId)
        {
            return await _bookRepo.DeleteBookFromDbAsync(bookId);
        }

        public async Task<ApiResponse<List<BookResponseDto>>> GetAllBooksAsync()
        {
            return await _bookRepo.GetAllBooksFromDbAsync();
        }
        public async Task<ApiResponse<BookResponseDto>> GetBookByIdAsync(int Id)
        {
            return await _bookRepo.GetBookByIdFromDb(Id);
        }
    }

}
