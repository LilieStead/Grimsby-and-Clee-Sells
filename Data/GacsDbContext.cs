using Grimsby_and_Clee_Sells.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace Grimsby_and_Clee_Sells.Data
{
    public class GacsDbContext :DbContext
    {
        public GacsDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions) 
        { 
        
        }

        // DbSet Here
       public  DbSet<User> Tbl_Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().HasKey(p=>p.users_id);
        }
    }
}
