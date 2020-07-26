using RestSharp;
using butterystrava.Strava.Responses;
using System.Net;
using System;

namespace butterystrava.Strava {
    public class Client {
        /*
        TODO:
        - client shouldnt know anything about buttery
        */

        private readonly RestClient _client;
        private readonly Strava.Settings _settings;
        public Client(Strava.Settings settings) {
            _settings = settings;
            _client = new RestClient("https://www.strava.com/api/v3");
        }

        private void Authorize() {
            /*
            This has to be done in the browser

            Step 1:
                https://www.strava.com/oauth/authorize
                ?client_id=51499
                &response_type=code
                &redirect_uri=http://localhost
                &approval_prompt=force
                &scope=activity:read_all,activity:write

                "Authorize buttery to connect to Strava"
                - View data about your public profile (required) 
                - View data about your private activities 
                - Upload your activities from buttery to Strava 
                [Authorize][Cancel]

            Step 2:
            on click Accept, redirect back to redirect_uri (https://ericbutera.com/buttery/code)
            read query string param `code`. Pass `code` into GetToken
            */
        }

        public IRestResponse<AuthorizationCode> AuthorizationCode(string code)
        {
            // Step 2:
            // Uses the redirect_uri from authorize to capture a `code` value. This code will
            // then be used to generate a access code token.
            //
            // https://www.strava.com/api/v3/oauth/token
            // ?client_id=51499
            // &client_secret=c716778950c4fe4bb8bd2b32f43f0076b95696c5  
            // &code=324dd39cb0410a6c25fe6faa1ee854880aae8bec  <-- set this into _Code
            // &grant_type=authorization_code
            var request = new RestRequest("oauth/token", Method.POST);
            request.AddParameter("client_id", _settings.ClientId);
            request.AddParameter("client_secret", _settings.ClientSecret);
            request.AddParameter("code", code, ParameterType.QueryString);
            request.AddParameter("grant_type", "authorization_code");
            var result = _client.Execute<AuthorizationCode>(request);

            // TOOD handle Bad request errors

            return result;

            /*
            when code expires:
            {
                "message": "Bad Request",
                "errors": [
                    {
                        "resource": "AuthorizationCode",
                        "field": "",
                        "code": "expired"
                    }
                ]
            }
            */
        }

        public IRestResponse<Athlete> Athlete(IToken token/*Models.Account account*/) {
            // https://www.strava.com/api/v3/athlete
            var request = new RestRequest("athlete");

            var result = Execute<Athlete>(token/*account*/, request);

            /* TODO err
            if (result.StatusCode == HttpStatusCode.OK) {
                // good to go
            } else {
                //when token expires: StatusCode: 401 -> RefreshToken
            } */

            return result;
        }

        public IRestResponse<Activities> Activities(/*int before, int after, int page, int per_page Models.Account account*/ IToken token) 
        {
            // before = epoch timestamp TODO make DateTime
            //https://developers.strava.com/playground/#/Activities/getLoggedInAthleteActivities
            var request = new RestRequest("athlete/activities", Method.GET);
            return Execute<Activities>(token/*account*/, request);
        }

        // Future:
        // https://developers.strava.com/playground/#/Activities/updateActivityById
        // https://developers.strava.com/playground/#/Athletes/updateLoggedInAthlete

        public bool Expired(IToken token) {
            //if (DateTime.Now >= token.DateExpiresAt) 
            // TODO
            return false;
        }

        public IRestResponse<T> Execute<T>(IToken token/*Models.Account account*/, IRestRequest request) {
            // Account is passed in for Token properties along with DateExpiresAt
            // Decouple by only passing in a token dto
            // How do we mark that token was updated?
            request.AddParameter("Authorization", $"Bearer {token.access_token}", ParameterType.HttpHeader);

            if (Expired(token)) 
                RefreshToken(token/*account.RefreshToken*/);

            var response = _client.Execute<T>(request);

            if (response.StatusCode == HttpStatusCode.Unauthorized) {
                // ERROR: Try refreshing token and execute request again.
                RefreshToken(token);
                return _client.Execute<T>(request);
            }

            return response;
        }

        /// Updates token by reference
        public IRestResponse<RefreshToken> RefreshToken(IToken token) {
            // is short-lived access token `expires_at` in past?
            // is `expire_at` < current_time?
            // yes: use token
            // no:
            // refresh

            var request = new RestRequest("oauth/token", Method.POST);
            request.AddParameter("client_id", _settings.ClientId);
            request.AddParameter("client_secret", _settings.ClientSecret);
            request.AddParameter("grant_type", "refresh_token");
            request.AddParameter("refresh_token", token.refresh_token);
            var response = _client.Execute<RefreshToken>(request);

            if (response.Data != null) {
                var refresh = response.Data;
                /* account.DateRefreshed = DateTime.Now; account.Token = refresh.access_token; account.RefreshToken = refresh.refresh_token; account.ExpiresIn = refresh.expires_in; account.ExpiresAt = refresh.expires_at; */
                token.access_token = refresh.access_token;
                token.refresh_token = refresh.refresh_token;
                token.expires_at = refresh.expires_at;
                token.expires_in = refresh.expires_in;
                token.NeedsSave = true;
                token.DateUpdated = DateTime.Now;
            }

            // TODO err
            return response;
        }
    }

    public interface IToken {
        string token_type {get;set;}
        string access_token {get;set;}
        long expires_at {get;set;}
        int expires_in {get;set;}
        string refresh_token {get;set;}
        bool NeedsSave {get;set;}
        DateTime DateUpdated {get;set;}
    }

    public class Token : IToken {
        public string token_type {get;set;} // = "Bearer"
        public string access_token {get;set;}
        public long expires_at {get;set;}
        public int expires_in {get;set;}
        public string refresh_token {get;set;}
        public bool NeedsSave {get;set;} = false;
        public DateTime DateUpdated {get;set;}
    }

}