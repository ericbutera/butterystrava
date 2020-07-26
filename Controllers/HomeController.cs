using Microsoft.AspNetCore.Mvc;
using butterystrava.Models;
using System.Linq;
using butterystrava.Strava;
using Microsoft.Extensions.Configuration;

namespace butterystrava.Controllers {

    public class HomeController : Controller {

        private readonly Settings _settings;
        private readonly Buttery.Buttery _buttery;
        private readonly Account _account;
        private readonly Client _client;
        private readonly string _redirectUri;

        public HomeController(ButteryContext context, Strava.Settings settings, Strava.Client client, IConfiguration config) {
            _settings = settings;
            _buttery = new Buttery.Buttery(context);
            _account = _buttery.GetAccount();
            _client = client;
            _redirectUri = config["StravaOauthRedirectUri"];
        }

        public IActionResult Index() {
            return View(new IndexModel() {
                Account = _account,
                AuthorizationUrl = _client.GetAuthUrl(_redirectUri)
            });
        }

        public RedirectToActionResult Code(string code) {
            // PRG back to home
            // step 2: use code to get auth token
            // http://localhost:5001/home/code?state=&code={sha1}&scope=read,activity:write,activity:read_all
            
            _account.Code = code;
            _buttery.Save(_account);

            return RedirectToAction("Index");
        }

    }
}