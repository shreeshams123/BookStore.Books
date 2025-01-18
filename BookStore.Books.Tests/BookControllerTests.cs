using BookStore.Books.Controllers;
using BookStore.Books.Interfaces;
using BookStore.Books.Models;
using BookStore.Books.Models.Dtos;
using BookStore.Books.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using System.Text;
using System.Text.Json;

namespace BookStore.Books.Tests
{
    [TestFixture]
    public class BookControllerTests
    {
        private Mock<IBookService> _mockBookService;
        private Mock<IDistributedCache> _mockDistributedCache;
        private BookContoller _controller;

        [SetUp]
        public void SetUp()
        {
            _mockBookService = new Mock<IBookService>();
            _mockDistributedCache = new Mock<IDistributedCache>();
            _controller = new BookContoller(_mockBookService.Object, _mockDistributedCache.Object);
        }

        [Test]
        public async Task CreateBook_ReturnsOk_WhenBookIsCreatedSuccessfully()
        {
            var fileContent = Encoding.UTF8.GetBytes("fake image");
            var bookDto = new CreateBookDto
            {
                Title = "New Book",
                Description = "A test book",
                Price = 19.99m,
                Image = new FormFile(new MemoryStream(fileContent), 0, fileContent.Length, "Image", "image.jpg")
            };

            var serviceResponse = new ApiResponse<Book>
            {
                Success = true,
                Message = "Book created",
                Data = new Book { }
            };

            _mockBookService.Setup(s => s.AddBookAsync(It.IsAny<CreateBookDto>(), It.IsAny<byte[]>()))
                .ReturnsAsync(serviceResponse);

            _mockDistributedCache.Setup(c => c.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await _controller.CreateBook(bookDto);

            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var response = okResult.Value as ApiResponse<Book>;
            Assert.IsNotNull(response);
            Assert.AreEqual(serviceResponse.Success, response.Success);
            Assert.AreEqual(serviceResponse.Message, response.Message);
            Assert.AreEqual(serviceResponse.Data, response.Data);

            _mockBookService.Verify(s => s.AddBookAsync(It.IsAny<CreateBookDto>(), It.IsAny<byte[]>()), Times.Once);
            _mockDistributedCache.Verify(c => c.RemoveAsync("allBooks", It.IsAny<CancellationToken>()), Times.Once);
        }


        [Test]
        public async Task UpdateBook_ReturnsOk_WhenBookIsUpdatedSuccessfully()
        {
            var bookDto = new UpdateBookDto
            {
                Title = "Updated Book",
                Price = 25.99m,
                Image = null
            };

            var serviceResponse = new ApiResponse<Book> { Success = true, Message = "Book updated" };
            _mockBookService.Setup(s => s.UpdateBookAsync(It.IsAny<int>(), It.IsAny<UpdateBookDto>(), It.IsAny<byte[]>()))
                .ReturnsAsync(serviceResponse);

            var result = await _controller.UpdateBook(1, bookDto) as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual(serviceResponse, result.Value);
        }

        [Test]
        public async Task DeleteBook_ReturnsOk_WhenBookIsDeletedSuccessfully()
        {
            var serviceResponse = new ApiResponse<Book> { Success = true, Message = "Book deleted" };
            _mockBookService.Setup(s => s.DeleteBookAsync(It.IsAny<int>())).ReturnsAsync(serviceResponse);

            var result = await _controller.DeleteBook(1) as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual(serviceResponse, result.Value);
        }

        [Test]
        public async Task GetAllBooks_ReturnsCachedBooks_WhenCacheIsAvailable()
        {
            var books = new List<BookResponseDto>
            {
                new BookResponseDto { BookId = 1, Title = "Cached Book 1" },
                new BookResponseDto { BookId = 2, Title = "Cached Book 2" }
            };

            var serializedBooks = JsonSerializer.Serialize(books);
            var byteArray = Encoding.UTF8.GetBytes(serializedBooks);

            var mockDistributedCache = new Mock<IDistributedCache>();
            mockDistributedCache.Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                                .ReturnsAsync(byteArray);

            _controller = new BookContoller(_mockBookService.Object, mockDistributedCache.Object);

            var result = await _controller.GetAllBooks() as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);

            var response = result.Value as ApiResponse<PaginatedResponse<BookResponseDto>>;
            Assert.IsNotNull(response);
            Assert.AreEqual(2, response.Data.TotalCount);
        }

        [Test]
        public async Task GetAllBooks_ReturnsFromDatabase_WhenCacheIsNotAvailable()
        {
            var books = new List<BookResponseDto>
            {
                new BookResponseDto { BookId = 1, Title = "DB Book 1" },
                new BookResponseDto { BookId = 2, Title = "DB Book 2" }
            };

            var serializedBooks = JsonSerializer.Serialize(books);
            var byteArray = Encoding.UTF8.GetBytes(serializedBooks);

            var mockDistributedCache = new Mock<IDistributedCache>();
            var serviceResponse = new ApiResponse<List<BookResponseDto>> { Success = true, Data = books };
            mockDistributedCache.Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                                           .ReturnsAsync(byteArray);

            _mockBookService.Setup(s => s.GetAllBooksAsync()).ReturnsAsync(serviceResponse);

            var result = await _controller.GetAllBooks() as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);

            var response = result.Value as ApiResponse<PaginatedResponse<BookResponseDto>>;
            Assert.IsNotNull(response);
            Assert.AreEqual(2, response.Data.TotalCount);
        }

        [Test]
        public async Task GetBookById_ReturnsOk_WhenBookIsFound()
        {
            var serviceResponse = new ApiResponse<BookResponseDto> { Success = true, Data = new BookResponseDto { BookId = 1, Title = "Test Book" } };
            _mockBookService.Setup(s => s.GetBookByIdAsync(It.IsAny<int>())).ReturnsAsync(serviceResponse);

            var result = await _controller.GetBookById(1) as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual(serviceResponse, result.Value);
        }

        [Test]
        public async Task GetBookById_ReturnsBadRequest_WhenBookIsNotFound()
        {
            var serviceResponse = new ApiResponse<BookResponseDto> { Success = false, Message = "Book not found" };
            _mockBookService.Setup(s => s.GetBookByIdAsync(It.IsAny<int>())).ReturnsAsync(serviceResponse);

            var result = await _controller.GetBookById(1) as BadRequestObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            Assert.AreEqual(serviceResponse, result.Value);
        }
    }
}
