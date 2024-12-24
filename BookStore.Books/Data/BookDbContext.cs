using BookStore.Books.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace BookStore.Books.Data
{
    public class BookDbContext : DbContext
    {
        public BookDbContext(DbContextOptions<BookDbContext> options) : base(options) { }

        public DbSet<Book> Books { get; set; }
    }
}
