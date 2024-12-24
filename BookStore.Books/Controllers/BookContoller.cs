using BookStore.Books.Interfaces;
using BookStore.Books.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Books.Controllers
{
    [ApiController]
    [Route("api/book")]
    public class BookContoller:ControllerBase
    {
        private readonly IBookService _bookService;
        public BookContoller(IBookService bookService)
        {
            _bookService = bookService;
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]  
        public async Task<IActionResult> CreateBook([FromBody] CreateBookDto bookDto)
        {
            var result = await _bookService.AddBookAsync(bookDto);
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateBook(int bookId, [FromBody] UpdateBookDto bookDto)
        {
            var result = await _bookService.UpdateBookAsync(bookId, bookDto);
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
            var result = await _bookService.GetAllBooksAsync();
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
