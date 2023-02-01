using Microsoft.EntityFrameworkCore;
using NLayer.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NLayer.Repository
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        //Best practice açısından productfeature ulaşıma kapatılarak products üzerinden işlemler gerçekleştirilir 
        public DbSet<ProductFeature> ProductFeatures { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            modelBuilder.Entity<ProductFeature>().HasData(new ProductFeature()
            {
                Id = 1,
                Color = "Kırmızı",
                Height = 100,
                Width = 200,
                ProductId = 1
            },
            new ProductFeature()
            {
                Id = 2,
                Color = "Mavi",
                Height = 300,
                Width = 500,
                ProductId = 2
            }


            );
            base.OnModelCreating(modelBuilder);
        }
        public override int SaveChanges()
        {
            foreach (var item in ChangeTracker.Entries())
            {
                if (item.Entity is BaseEntity entityRefference)
                {
                    switch (item.State)
                    {
                        case EntityState.Added:
                            entityRefference.CreatedDate = DateTime.Now;
                            break;
                        case EntityState.Modified:
                            entityRefference.UpdatedDate = DateTime.Now; break;
                    }
                }
            }
            return base.SaveChanges();
        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var item in ChangeTracker.Entries())
            {
                if (item.Entity is BaseEntity entityRefference)
                {
                    switch (item.State)
                    {
                        case EntityState.Added:
                            entityRefference.CreatedDate = DateTime.Now;
                            break;
                        case EntityState.Modified:
                            Entry(entityRefference).Property(x => x.CreatedDate).IsModified = false;
                            entityRefference.UpdatedDate = DateTime.Now; break;
                    }
                }
            }
            return base.SaveChangesAsync();
        }
    }
}
