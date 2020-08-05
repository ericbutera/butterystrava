using System;
using System.Linq;
using butterystrava.Models;

namespace butterystrava.Buttery {
    // Refactor better namespace & name
    public class Buttery {
        private ButteryContext _context;

        public Buttery(ButteryContext context) {
            _context = context;
        }

        // Convert buttery Account into a Strava.IToken
        public butterystrava.Strava.IToken GetStravaToken(User account) {
            return new butterystrava.Strava.Token {
                access_token = account.Token,
                refresh_token = account.RefreshToken,
                expires_at = account.ExpiresAt,
                expires_in = account.ExpiresIn
            };
        }

        // Map Strava token to buttery Account
        public void Map(Strava.IToken token, User account) {
            account.DateRefreshed = token.DateUpdated;
            account.Token = token.access_token;
            account.RefreshToken = token.refresh_token;
            account.DateExpiresAt = System.DateTimeOffset.FromUnixTimeSeconds(token.expires_at);
            account.DateExpiresIn = (new DateTime()).AddSeconds(token.expires_in);
        }

        public User LoadOrCreate(string username) {
            var account = _context.Accounts.FirstOrDefault(u => u.AthleteUsername == username);

            if (account == null) {
                account = new User(){ 
                    AthleteUsername = username
                };
                _context.Accounts.Add(account);

                _context.SaveChanges();
            }
            
            return account;
        }

        public User Load(string username) {
            // account requried to exist
            return _context.Accounts.FirstOrDefault(u => u.AthleteUsername == username);
        }

        /// Save Token to Account
        public bool Save(User account, Strava.IToken token) 
        {
            if (string.IsNullOrWhiteSpace(token.access_token)) 
                throw new ArgumentException("AccessToken is required to save");

            Map(token, account);
            token.NeedsSave = false;

            return _context.SaveChanges() > 0;
        }

        public bool Save(User account)
        {
            // attach if not connected
            //_context.Accounts.Update(account)
            // DateUpdated = DateTime.Now
            return _context.SaveChanges() > 0;
        }
    }
}