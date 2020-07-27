using Microsoft.AspNetCore.Mvc;
using butterystrava.Models;
using System.Linq;
using butterystrava.Strava;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;

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

        public RedirectToActionResult Index() {
            var username = HttpContext.Session.GetString("username");

            if (!string.IsNullOrWhiteSpace(username))
                return RedirectToAction("Welcome");

            return RedirectToAction("Login");
        }

        public IActionResult Login()
        {
            ViewData["AuthorizationUrl"] = _client.GetAuthUrl(_redirectUri);
            return View();
        }

        public RedirectToActionResult SessionTest()
        {
            return RedirectToAction("Index");
        }

        public RedirectToActionResult Code(string code) {
            // PRG back to home
            // step 2: use code to get auth token
            // http://localhost:5001/home/code?state=&code={sha1}&scope=read,activity:write,activity:read_all
            
            HttpContext.Session.SetString("code", code);

            _account.Code = code;
            _buttery.Save(_account);

            return RedirectToAction("GetToken");
        }

        public IActionResult GetToken() {
            ViewData["username"] = HttpContext.Session.GetString("username");
            ViewData["code"] = HttpContext.Session.GetString("code");
            return View();
        }

        public RedirectToActionResult AuthorizationCode() {
            var result = _client.AuthorizationCode(_account.Code); 

            var username = result.Data.athlete.username;

            HttpContext.Session.SetString("username", username);
            _account.AthleteUsername = username;
            _buttery.Save(_account);

            return RedirectToAction("Welcome");
        }

        public IActionResult Welcome() {
            var username = HttpContext.Session.GetString("username");

            if (string.IsNullOrWhiteSpace(username))
                return RedirectToAction("Login");

            ViewData["username"] = username;

            return View(new IndexModel() {
                Account = _account,
            });
        }
    }
}