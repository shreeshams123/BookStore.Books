using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore.Books.Models.Entities
{
    public class Book
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        
        public int BookId { get; set; }
        [Required]
        public string Title { get; set; }

        [Required]
        public string Author { get; set; }
        [Required] 
        public string Description { get; set; }
        [Required]
        public int StockQuantity {  get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public string ImageUrl { get; set; }
    }
}
