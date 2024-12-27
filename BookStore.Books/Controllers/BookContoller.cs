using BookStore.Books.Interfaces;
using BookStore.Books.Models;
using BookStore.Books.Models.Dtos;
using BookStore.Books.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace BookStore.Books.Controllers
{
    [ApiController]
    [Route("api/book")]
    public class BookContoller : ControllerBase
    {
        private readonly IBookService _bookService;
        private readonly IDistributedCache _distributedCache;
        public BookContoller(IBookService bookService,IDistributedCache distributedCache)
        {
            _bookService = bookService;
            _distributedCache = distributedCache;
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateBook(CreateBookDto bookDto)  
        {
            if (bookDto.Image == null || bookDto.Image.Length == 0)
            {
                return BadRequest("No image uploaded.");
            }

            byte[] imageBytes;
            using (var memoryStream = new MemoryStream())
            {
                await bookDto.Image.CopyToAsync(memoryStream);
                imageBytes = memoryStream.ToArray();
            }

            var result = await _bookService.AddBookAsync(bookDto, imageBytes);

            await _distributedCache.RemoveAsync($"allBooks");

            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }


        [HttpPut("{bookId}")]
        [Authorize]
        public async Task<IActionResult> UpdateBook(int bookId,  UpdateBookDto bookDto)
        {
            byte[] imageBytes = null;
            if (bookDto.Image != null && bookDto.Image.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await bookDto.Image.CopyToAsync(memoryStream);
                    imageBytes = memoryStream.ToArray();
                }
            }

            var result = await _bookService.UpdateBookAsync(bookId, bookDto, imageBytes);
            await _distributedCache.RemoveAsync($"allBooks");
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var result = await _bookService.DeleteBookAsync(id);
            await _distributedCache.RemoveAsync($"allBooks");
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBooks()
        {
            var cacheBooks = await _distributedCache.GetStringAsync("allBooks");
            if (!string.IsNullOrEmpty(cacheBooks))
            {
                var booksFromCache = System.Text.Json.JsonSerializer.Deserialize<IEnumerable<Book>>(cacheBooks);

                var apiResponse = new ApiResponse<IEnumerable<Book>>
                {
                    Success = true,
                    Message = "Books retrieved successfully from cache",
                    Data = booksFromCache
                };
                return Ok(apiResponse);
            }

            var result = await _bookService.GetAllBooksAsync();
            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }


        [HttpGet("{Id}")]
        public async Task<IActionResult> GetBookById(int Id)
        {
            var result=await _bookService.GetBookByIdAsync(Id);
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
    }
}
