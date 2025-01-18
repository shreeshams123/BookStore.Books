using BookStore.Books.Interfaces;
using BookStore.Books.Models;
using BookStore.Books.Models.Dtos;
using BookStore.Books.Models.Entities;
using BookStore.Books.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Books.Tests
{
    [TestFixture]
    public class BookServiceTests
    {
        private Mock<IBookRepo> _mockBookRepo;
        private BookService _bookService;

        [SetUp]
        public void SetUp()
        {
            _mockBookRepo = new Mock<IBookRepo>();
            _bookService = new BookService(_mockBookRepo.Object);
        }

        [Test]
        public async Task AddBookAsync_ReturnsApiResponse_WhenBookIsAddedSuccessfully()
        {
            var bookDto = new CreateBookDto
            {
                Title = "Test Book",
                Description = "Test Description",
                Price = 29.99m
            };
            var imageBytes = new byte[] { 0x20, 0x21 };

            var expectedResponse = new ApiResponse<Book>
            {
                Success = true,
                Message = "Book added successfully",
                Data = new Book { Title = "Test Book", Description = "Test Description", Price = 29.99m }
            };

            _mockBookRepo
                .Setup(repo => repo.AddBookToDbAsync(bookDto, imageBytes))
                .ReturnsAsync(expectedResponse);

            var result = await _bookService.AddBookAsync(bookDto, imageBytes);

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResponse.Success, result.Success);
            Assert.AreEqual(expectedResponse.Message, result.Message);
            Assert.AreEqual(expectedResponse.Data.Title, result.Data.Title);
            _mockBookRepo.Verify(repo => repo.AddBookToDbAsync(bookDto, imageBytes), Times.Once);
        }

        [Test]
        public async Task UpdateBookAsync_ReturnsApiResponse_WhenBookIsUpdatedSuccessfully()
        {
            var bookId = 1;
            var bookDto = new UpdateBookDto
            {
                Title = "Updated Book",
                Price = 19.99m
            };
            var imageBytes = new byte[] { 0x30, 0x31 };

            var expectedResponse = new ApiResponse<Book>
            {
                Success = true,
                Message = "Book updated successfully"
            };

            _mockBookRepo
                .Setup(repo => repo.UpdateBookInDbAsync(bookId, bookDto, imageBytes))
                .ReturnsAsync(expectedResponse);

            var result = await _bookService.UpdateBookAsync(bookId, bookDto, imageBytes);

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResponse.Success, result.Success);
            Assert.AreEqual(expectedResponse.Message, result.Message);
            _mockBookRepo.Verify(repo => repo.UpdateBookInDbAsync(bookId, bookDto, imageBytes), Times.Once);
        }

        [Test]
        public async Task DeleteBookAsync_ReturnsApiResponse_WhenBookIsDeletedSuccessfully()
        {
            var bookId = 1;
            var expectedResponse = new ApiResponse<Book>
            {
                Success = true,
                Message = "Book deleted successfully"
            };

            _mockBookRepo
                .Setup(repo => repo.DeleteBookFromDbAsync(bookId))
                .ReturnsAsync(expectedResponse);

            var result = await _bookService.DeleteBookAsync(bookId);

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResponse.Success, result.Success);
            Assert.AreEqual(expectedResponse.Message, result.Message);
            _mockBookRepo.Verify(repo => repo.DeleteBookFromDbAsync(bookId), Times.Once);
        }

        [Test]
        public async Task GetAllBooksAsync_ReturnsApiResponse_WithListOfBooks()
        {
            var expectedBooks = new List<BookResponseDto>
            {
                new BookResponseDto { BookId = 1, Title = "Book 1" },
                new BookResponseDto { BookId = 2, Title = "Book 2" }
            };
            var expectedResponse = new ApiResponse<List<BookResponseDto>>
            {
                Success = true,
                Data = expectedBooks
            };

            _mockBookRepo
                .Setup(repo => repo.GetAllBooksFromDbAsync())
                .ReturnsAsync(expectedResponse);

            var result = await _bookService.GetAllBooksAsync();

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResponse.Success, result.Success);
            Assert.AreEqual(expectedResponse.Data.Count, result.Data.Count);
            _mockBookRepo.Verify(repo => repo.GetAllBooksFromDbAsync(), Times.Once);
        }

        [Test]
        public async Task GetBookByIdAsync_ReturnsApiResponse_WhenBookIsFound()
        {
            var bookId = 1;
            var expectedResponse = new ApiResponse<BookResponseDto>
            {
                Success = true,
                Data = new BookResponseDto { BookId = bookId, Title = "Found Book" }
            };

            _mockBookRepo
                .Setup(repo => repo.GetBookByIdFromDb(bookId))
                .ReturnsAsync(expectedResponse);

            var result = await _bookService.GetBookByIdAsync(bookId);

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResponse.Success, result.Success);
            Assert.AreEqual(expectedResponse.Data.BookId, result.Data.BookId);
            _mockBookRepo.Verify(repo => repo.GetBookByIdFromDb(bookId), Times.Once);
        }
    }
}
