﻿using Grimsby_and_Clee_Sells.Models.Domain;
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().HasKey(p=>p.users_id);
            modelBuilder.Entity<Category>().HasKey(p => p.category_id);
            modelBuilder.Entity<Status>().HasKey(p=>p.status_id);
            modelBuilder.Entity<Product>().HasKey(p=>p.product_id);


            //modelBuilder.Entity<CategoryDTO>().HasKey(p => p.category_id);
           // modelBuilder.Entity<StatusDTO>().HasKey(p => p.status_id);
           // modelBuilder.Entity<UserDTO>().HasKey(p => p.users_id);

            modelBuilder.Entity<Product>().HasOne(p => p.Category).WithMany().HasForeignKey(p => p.product_category);
            modelBuilder.Entity<Product>().HasOne(p => p.User).WithMany().HasForeignKey(p => p.product_userid);
            modelBuilder.Entity<Product>().HasOne(p => p.Status).WithMany().HasForeignKey(p => p.product_status);

        }
    }
}
