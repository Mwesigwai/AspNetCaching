using AspNetCaching.Models;
using Microsoft.EntityFrameworkCore;

namespace AspNetCaching.Data
{
    public class DocumentDbContext(DbContextOptions<DocumentDbContext> options)
        :DbContext(options)
    {
        public DbSet<Document> Documents { get; set; }
    }
}
