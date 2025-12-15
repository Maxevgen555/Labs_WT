using Labs.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Labs.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Dish> Dishes { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Настройка отношений
            modelBuilder.Entity<Dish>()
                .HasOne(d => d.Category)
                .WithMany()
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
