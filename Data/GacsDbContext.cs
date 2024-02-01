using Grimsby_and_Clee_Sells.Models.Domain;
using Grimsby_and_Clee_Sells.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;

namespace Grimsby_and_Clee_Sells.Data
{
    public class GacsDbContext :DbContext
    {
        public GacsDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions) 
        { 
        
        }

        // DbSet Here
       public  DbSet<User> Tbl_Users { get; set; }
       public DbSet<Category> Tbl_Category { get; set; }

        public DbSet<Status> Tbl_Status { get; set; }

        public DbSet<Product> Tbl_Product { get; set; }

        public DbSet<Productimg> Tbl_Productimg { get; set; }

        public DbSet<Admin> Tbl_Admin { get; set; }

        public DbSet<Cartitem> Tbl_Cart { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //P keys
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().HasKey(p=>p.users_id);
            modelBuilder.Entity<Category>().HasKey(p => p.category_id);
            modelBuilder.Entity<Status>().HasKey(p=>p.status_id);
            modelBuilder.Entity<Product>().HasKey(p=>p.product_id);
            modelBuilder.Entity<Productimg>().HasKey(p => p.productimg_id);
            modelBuilder.Entity<Admin>().HasKey(p => p.admin_id);
            // use this example for composite keys
            modelBuilder.Entity<Cartitem>().HasKey(p => new { p.cart_userid, p.cart_productid});



            //F key 
            modelBuilder.Entity<Product>().HasOne(p => p.Category).WithMany().HasForeignKey(p => p.product_category);
            modelBuilder.Entity<Product>().HasOne(p => p.User).WithMany().HasForeignKey(p => p.product_userid);
            modelBuilder.Entity<Product>().HasOne(p => p.Status).WithMany().HasForeignKey(p => p.product_status);
            modelBuilder.Entity<Productimg>().HasOne(p => p.Product).WithMany().HasForeignKey(p => p.productimg_productid);
            modelBuilder.Entity<Cartitem>().HasOne(p => p.User).WithMany().HasForeignKey(p => p.cart_userid);
            modelBuilder.Entity<Cartitem>().HasOne(p => p.product).WithMany().HasForeignKey(p => p.cart_productid);


        }
    }
}
