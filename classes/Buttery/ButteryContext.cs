using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;


namespace butterystrava.Buttery {

    public class ButteryContext : IdentityDbContext<User> {
        public DbSet<User> Accounts {get;set;}

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=buttery.db");
    }

}