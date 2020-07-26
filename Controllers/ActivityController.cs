using System;
using System.Net;
using butterystrava.Models;
using Microsoft.AspNetCore.Mvc;

namespace butterystrava.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ActivityController : ControllerBase
    {

        private readonly Strava.Client _client;
        private readonly Buttery.Buttery _buttery;
        private readonly Account _account;

        public ActivityController(Strava.Client client, Models.ButteryContext context) {
            _client = client;

            _buttery = new Buttery.Buttery(context);
            _account = _buttery.GetAccount();
        }

        internal class ApiError : Exception {
            public HttpStatusCode StatusCode {get;set;}
            public string ErrorMessage {get;set;}
            public string Content {get;set;}
        }

        [Route("authorization-code")]
        public dynamic AuthorizationCode() {
            var result = _client.AuthorizationCode(_account.Code); 

            if (result.StatusCode == HttpStatusCode.OK && result.Data != null) {
                _buttery.Save(_account, result.Data);

                return new {
                    Account = _account
                };
            }

            throw new ApiError() { StatusCode = result.StatusCode, ErrorMessage = result.ErrorMessage, Content = result.Content };
        }

        [Route("refresh-token")]
        public dynamic RefreshToken() {
            var token =_buttery.GetStravaToken(_account);
            var result = _client.RefreshToken(token);

            if (result.StatusCode == HttpStatusCode.OK) {
                _buttery.Save(_account, token);

                return new {
                    Account = _account,
                };
            }

            throw new ApiError() { StatusCode = result.StatusCode, ErrorMessage = result.ErrorMessage, Content = result.Content };
        }

        [Route("athlete")]
        public dynamic Athlete() {
            var token = _buttery.GetStravaToken(_account);
            var result = _client.Athlete(token);

            if (result.StatusCode == HttpStatusCode.OK) {
                _buttery.Save(_account, token);

                return new {
                    Athlete = result.Data
                };
            }

            throw new ApiError() { StatusCode = result.StatusCode, ErrorMessage = result.ErrorMessage, Content = result.Content };
        }

        [Route("activities")]
        public dynamic Activities()
        {
            var token = _buttery.GetStravaToken(_account);
            var result = _client.Activities(token);

            if (result.StatusCode == HttpStatusCode.OK) {
                _buttery.Save(_account, token);

                return new {
                    Activities = result.Data
                };
            }

            throw new ApiError() { StatusCode = result.StatusCode, ErrorMessage = result.ErrorMessage, Content = result.Content };
        }
    }

}