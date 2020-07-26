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
        public butterystrava.Strava.IToken GetStravaToken(Account account) {
            return new butterystrava.Strava.Token {
                access_token = account.Token,
                refresh_token = account.RefreshToken,
                expires_at = account.ExpiresAt,
                expires_in = account.ExpiresIn
            };
        }

        // Map Strava token to buttery Account
        public void Map(Strava.IToken token, Account account) {
            account.DateRefreshed = token.DateUpdated;
            account.Token = token.access_token;
            account.RefreshToken = token.refresh_token;
            account.DateExpiresAt = System.DateTimeOffset.FromUnixTimeSeconds(token.expires_at);
            account.DateExpiresIn = (new DateTime()).AddSeconds(token.expires_in);
        }

        // TODO sessions
        public Account GetAccount() {
            var account = _context.Accounts.FirstOrDefault();
            if (account == null) {
                account = new Account(){ };
                _context.Accounts.Add(account);
                _context.SaveChanges();
            }

            return account;
        }

        /// Save Token to Account
        public bool Save(Account account, Strava.IToken token) 
        {
            if (string.IsNullOrWhiteSpace(token.access_token)) 
                throw new ArgumentException("AccessToken is required to save");

            Map(token, account);
            token.NeedsSave = false;

            return _context.SaveChanges() > 0;
        }

        public bool Save(Account account)
        {
            // attach if not connected
            //_context.Accounts.Update(account)
            // DateUpdated = DateTime.Now
            return _context.SaveChanges() > 0;
        }
    }
}