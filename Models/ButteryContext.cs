using System;
using Microsoft.EntityFrameworkCore;

namespace butterystrava.Models {

    public class ButteryContext : DbContext {
        public DbSet<Account> Accounts {get;set;}

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=buttery.db");
    }

    public class Account {
        // this should probably be normalized at some point
        public int AccountId {get;set;}
        public string Token {get;set;} // access_token
        public string RefreshToken {get;set;}
        public string Code {get;set;}
        public DateTime DateRefreshed {get;set;}
        public long ExpiresAt {get;set;}
        public int ExpiresIn {get;set;}
        public DateTime DateExpiresIn { get;set; }
        public DateTimeOffset DateExpiresAt {get;set; }
        public string AthleteUsername { get; set; } // strava username
        public long AthleteId { get; set; } // strava id
        public string Hash { get; set; }
        public string Salt {get;set;}
    }

}