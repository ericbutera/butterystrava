using Microsoft.AspNetCore.Mvc;

//using Strava;

namespace butterystrava.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ActivityController : ControllerBase
    {

        private readonly StravaSettings _settings;
        public ActivityController(StravaSettings settings) {
            _settings = settings;
        }
            
        public dynamic Get()
        {

            return new {
                 Client =_settings.ClientId,
                 Sec = _settings.ClientSecret
            };
        }
    }

}