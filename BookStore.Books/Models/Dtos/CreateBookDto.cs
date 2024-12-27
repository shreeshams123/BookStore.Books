using System.ComponentModel.DataAnnotations;

namespace BookStore.Books.Models.Dtos
{
    public class CreateBookDto
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Author { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int StockQuantity { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public IFormFile Image { get; set; }
    }
}
