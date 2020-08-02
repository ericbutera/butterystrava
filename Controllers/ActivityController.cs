using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http; // todo get rid of session

namespace butterystrava.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ActivityController : ControllerBase
    {

        private readonly Strava.Client _client;
        private readonly Buttery.Buttery _buttery;
        //private readonly Account _account;

        public ActivityController(Strava.Client client, Models.ButteryContext context) 
        {
            _client = client;
            _buttery = new Buttery.Buttery(context);
            //_account = _buttery.Get();
        }

        internal class ApiError : Exception {
            public HttpStatusCode StatusCode {get;set;}
            public string ErrorMessage {get;set;}
            public string Content {get;set;}
        }

        [Route("authorization-code")]
        public dynamic AuthorizationCode(string code) 
        {
            var result = _client.AuthorizationCode(code); 

            if (result.StatusCode == HttpStatusCode.OK && result.Data != null) {
                var authorization = result.Data;
                var account = _buttery.LoadOrCreate(result.Data.athlete.username);

                account.AthleteUsername = authorization.athlete.username;
                account.AthleteId = authorization.athlete.id;

                _buttery.Save(account, authorization); //_buttery.Save(account, result.Data);

                return new {
                    Account = account
                };
            }

            throw new ApiError() { StatusCode = result.StatusCode, ErrorMessage = result.ErrorMessage, Content = result.Content };
        }

        [Route("refresh-token")]
        public dynamic RefreshToken() 
        {
            /*
            How to get account?
            - web api isn't supposed to use sessions. it uses bearer token auth or simple user/pass
            - ok, try simple user/pass
              - we don't have a password. so user needs to finish creating an account after strava auth
            */
            var username = HttpContext.Session.GetString("username"); // TODO get rid of session
            var account = _buttery.Load(username);

            var token =_buttery.GetStravaToken(account);
            var result = _client.RefreshToken(token);

            if (result.StatusCode == HttpStatusCode.OK) {
                _buttery.Save(account, token);

                return new {
                    Account = account,
                };
            }

            throw new ApiError() { StatusCode = result.StatusCode, ErrorMessage = result.ErrorMessage, Content = result.Content };
        }

        [Route("athlete")]
        public dynamic Athlete() 
        {
            var username = HttpContext.Session.GetString("username"); // TODO get rid of session
            var account = _buttery.Load(username);

            var token = _buttery.GetStravaToken(account);
            var result = _client.Athlete(token);

            if (result.StatusCode == HttpStatusCode.OK) {
                _buttery.Save(account, token);

                return new {
                    Athlete = result.Data
                };
            }

            // "{\"message\":\"Authorization Error\",\"errors\":[{\"resource\":\"Athlete\",\"field\":\"access_token\",\"code\":\"invalid\"}]}"
            throw new ApiError() { StatusCode = result.StatusCode, ErrorMessage = result.ErrorMessage, Content = result.Content };
        }

        [Route("activities")]
        public dynamic Activities()
        {
            var username = HttpContext.Session.GetString("username"); // TODO get rid of session
            var account = _buttery.Load(username);

            var token = _buttery.GetStravaToken(account);
            var result = _client.Activities(token);

            if (result.StatusCode == HttpStatusCode.OK) {
                _buttery.Save(account, token);

                return new {
                    Activities = result.Data
                };
            }

            throw new ApiError() { StatusCode = result.StatusCode, ErrorMessage = result.ErrorMessage, Content = result.Content };
        }
    }

}