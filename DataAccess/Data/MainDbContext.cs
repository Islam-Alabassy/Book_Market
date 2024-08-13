using Microsoft.EntityFrameworkCore;
using Models;

namespace DataAccess
{
    public class MainDbContext : DbContext
    {
        private readonly DbContextOptions<MainDbContext> options;

        public MainDbContext(DbContextOptions<MainDbContext> options):base(options) 
        {
            this.options = options;
        }

        public virtual DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().
                HasData(
                new Category() { CategoryId=1,Name="Action",DisplayOrder=1},
                new Category() { CategoryId=2,Name="Science Fiction",DisplayOrder=2},
                new Category() { CategoryId=3,Name="History",DisplayOrder=3}
                );
            base.OnModelCreating(modelBuilder);
        }
    }
}
