namespace BookStore.Books.Models.Dtos
{
    public class BookResponseDto
    {
        
            public int BookId { get; set; }
            public string Title { get; set; }
            public string Author { get; set; }
            public string Description { get; set; }
            public int StockQuantity { get; set; }
            public decimal Price { get; set; }
            public string Image { get; set; }  
        
    }
    public class PaginatedResponse<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
    }

}
