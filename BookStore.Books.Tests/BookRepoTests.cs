using BookStore.Books.Data;
using BookStore.Books.Models.Dtos;
using BookStore.Books.Models.Entities;
using BookStore.Books.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Moq;


namespace BookStore.Books.Tests
{
    [TestFixture]
    public class BookRepoTests
    {
        private BookDbContext _dbContext;
        private BookRepo _bookRepo;
        private DbContextOptions<BookDbContext> _dbContextOptions;
        private Mock<IDistributedCache> _mockDistributedCache;

        [SetUp]
        public void SetUp()
        {
            _dbContextOptions = new DbContextOptionsBuilder<BookDbContext>()
                                .UseInMemoryDatabase(databaseName: "TestBookDb")
                                .Options;

            _dbContext = new BookDbContext(_dbContextOptions);
            _mockDistributedCache = new Mock<IDistributedCache>();

            _bookRepo = new BookRepo(_dbContext, _mockDistributedCache.Object);
            _dbContext.Set<Book>().RemoveRange(_dbContext.Set<Book>());
            _dbContext.SaveChangesAsync().Wait();
        }

        [Test]
        public async Task AddBookToDbAsync_ShouldReturnSuccessResponse()
        {
            var bookDto = new CreateBookDto
            {
                Title = "Test Book",
                Author = "Author Name",
                Description = "Description",
                StockQuantity = 10,
                Price = 19.99m
            };
            var imageBytes = new byte[] { 0x1, 0x2, 0x3 };

            var result = await _bookRepo.AddBookToDbAsync(bookDto, imageBytes);

            Assert.IsTrue(result.Success, "Expected Success to be true.");
            Assert.AreEqual("Book created successfully", result.Message, "Expected success message to match.");
            Assert.IsNotNull(result.Data, "Expected Data to be not null.");
            Assert.AreEqual(bookDto.Title, result.Data.Title, "Expected Title to match.");
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Dispose();
        }

        public void Dispose()
        {
            TearDown();
        }

        [Test]
        public async Task UpdateBookInDbAsync_ShouldReturnSuccessResponse_WhenBookExists()
        {
            var bookId = 1;
            var bookDto = new UpdateBookDto
            {
                Title = "Updated Title",
                Author = "Updated Author"
            };

            var book = new Book
            {
                BookId = bookId,
                Title = "Original Title",
                Author = "Original Author",
                Description = "Original Description",
                StockQuantity = 10,
                Price = 19.99m,
                Image = new byte[] { 0x1, 0x2, 0x3 }
            };

            _dbContext.Set<Book>().Add(book);
            await _dbContext.SaveChangesAsync();

            var mockDistributedCache = new Mock<IDistributedCache>();

            var bookRepo = new BookRepo(_dbContext, mockDistributedCache.Object);

            var result = await bookRepo.UpdateBookInDbAsync(bookId, bookDto);

            Assert.IsTrue(result.Success, "Expected Success to be true.");
            Assert.AreEqual("Book updated successfully", result.Message, "Expected success message to match.");
            Assert.AreEqual(bookDto.Title, result.Data.Title, "Expected Title to match.");

            var updatedBook = await _dbContext.Set<Book>().FindAsync(bookId);
            Assert.AreEqual(bookDto.Title, updatedBook.Title, "Expected book title to be updated.");
            Assert.AreEqual(bookDto.Author, updatedBook.Author, "Expected book author to be updated.");
        }

        [Test]
        public async Task DeleteBookFromDbAsync_ShouldReturnSuccessResponse_WhenBookExists()
        {
            var bookId = 1;
            var book = new Book
            {
                BookId = bookId,
                Title = "Test Book",
                Author = "Author Name",
                Description = "Test Description",
                StockQuantity = 10,
                Price = 19.99m,
                Image = new byte[] { 0x1, 0x2, 0x3 }
            };

            _dbContext.Set<Book>().Add(book);
            await _dbContext.SaveChangesAsync();

            var result = await _bookRepo.DeleteBookFromDbAsync(bookId);

            Assert.IsTrue(result.Success, "Expected Success to be true.");
            Assert.AreEqual("Book deleted successfully", result.Message, "Expected success message to match.");

            var deletedBook = await _dbContext.Set<Book>().FindAsync(bookId);
            Assert.IsNull(deletedBook, "Expected book to be deleted from the database.");
        }

        [Test]
        public async Task GetAllBooksFromDbAsync_ShouldReturnBooks_WhenBooksExistInDb()
        {
            var books = new[]
            {
                new Book { BookId = 1, Title = "Book 1", Author = "Author 1", Description = "Description 1", StockQuantity = 10, Price = 19.99m, Image = new byte[] { 0x1, 0x2 } },
                new Book { BookId = 2, Title = "Book 2", Author = "Author 2", Description = "Description 2", StockQuantity = 5, Price = 29.99m, Image = new byte[] { 0x1, 0x2 } }
            };

            await _dbContext.Set<Book>().AddRangeAsync(books);
            await _dbContext.SaveChangesAsync();

            var result = await _bookRepo.GetAllBooksFromDbAsync();

            Assert.IsTrue(result.Success);
            Assert.AreEqual(2, result.Data.Count);
            Assert.AreEqual("Books retrieved successfully", result.Message);

            var retrievedBooks = await _dbContext.Set<Book>().ToListAsync();
            Assert.AreEqual(2, retrievedBooks.Count, "Expected 2 books in the database.");
        }

        [Test]
        public async Task GetBookByIdFromDb_ShouldReturnSuccessResponse_WhenBookExists()
        {
            var bookId = 1;
            var book = new Book
            {
                BookId = bookId,
                Title = "Test Book",
                Author = "Author",
                Description = "Description",
                StockQuantity = 10,
                Price = 19.99m,
                Image = new byte[] { 0x1, 0x2 }
            };

            _dbContext.Set<Book>().Add(book);
            await _dbContext.SaveChangesAsync();

            var result = await _bookRepo.GetBookByIdFromDb(bookId);

            Assert.IsTrue(result.Success);
            Assert.AreEqual("Book retrieved successfully", result.Message);
            Assert.AreEqual(bookId, result.Data.BookId);

            var retrievedBook = await _dbContext.Set<Book>().FindAsync(bookId);
            Assert.IsNotNull(retrievedBook, "Expected book to be found in the database.");
            Assert.AreEqual(bookId, retrievedBook.BookId, "Expected book to match the provided ID.");
        }
    }
}
