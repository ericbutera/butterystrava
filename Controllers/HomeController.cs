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
        private readonly Client _client;
        private readonly string _redirectUri;

        private string _username {
            get {
                return HttpContext.Session.GetString("username");
            }
            set {
                if (value == null)
                    HttpContext.Session.Remove("username");
                else 
                    HttpContext.Session.SetString("username", value);
            }
        }
        private string _code {
            get {
                return HttpContext.Session.GetString("code");
            }
            set {
                HttpContext.Session.SetString("code", value);
            }
        }

        public HomeController(ButteryContext context, Strava.Settings settings, Strava.Client client, IConfiguration config) {
            _settings = settings;
            _buttery = new Buttery.Buttery(context);
            _client = client;
            _redirectUri = config["StravaOauthRedirectUri"];
        }

        public RedirectToActionResult Index() {
            if (!string.IsNullOrWhiteSpace(_username))
                return RedirectToAction("Welcome");

            return RedirectToAction("Login");
        }

        public IActionResult Login()
        {
            if (!string.IsNullOrWhiteSpace(_username))
                return RedirectToAction("Welcome");

            ViewData["AuthorizationUrl"] = _client.GetAuthUrl(_redirectUri);
            return View();
        }

        public RedirectToActionResult Logout() 
        {
            _username = null;
            return RedirectToAction("Index");
        }

        public RedirectToActionResult Authorize(string username, string password) {
            // refactor auth into something nicer
            if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password)) {
                var account = _buttery.Load(username);
                if (account == null) //"Account not found. Use Login with Strava"
                    return RedirectToAction("Login");

                var hash = new HashLibrary.HashedPassword(account.Hash, account.Salt);
                if (hash.Check(password)) {
                    // success
                    _username = account.AthleteUsername;
                    return RedirectToAction("Welcome");
                } else {
                    // "Error with username or password"
                    return RedirectToAction("Login");
                }
            }

            return RedirectToAction("Login");
        }

        public RedirectToActionResult Password(string password) {
            var account = _buttery.Load(_username);

            var hash = HashLibrary.HashedPassword.New(password);
            account.Hash = hash.Hash;
            account.Salt = hash.Salt;

            _buttery.Save(account);

            return RedirectToAction("Welcome");
        }

        public RedirectToActionResult Code(string code) {
            // PRG back to home
            // step 2: use code to get auth token
            // http://localhost:5001/home/code?state=&code={sha1}&scope=read,activity:write,activity:read_all
            
            _code = code;
            // need to save code for later ?
            //_account.Code = code;
            //_buttery.Save(_account);


            return RedirectToAction("GetToken");
        }

        public IActionResult GetToken() {
            ViewData["username"] = _username;
            ViewData["code"] = _code;
            return View();
        }

        public RedirectToActionResult AuthorizationCode() {
            var code = _code;

            if (string.IsNullOrEmpty(code))
                return RedirectToAction("Login");

            var result = _client.AuthorizationCode(code); 

            var username = result.Data.athlete.username;
            _username = username;

            var account = _buttery.LoadOrCreate(username);
            account.Code = code;
            account.AthleteUsername = _username;

            _buttery.Save(account);

            return RedirectToAction("Welcome");
        }

        public IActionResult Welcome() {
            var username = _username;

            if (string.IsNullOrWhiteSpace(username))
                return RedirectToAction("Login");

            ViewData["username"] = username;


            var hash = HashLibrary.HashedPassword.New("moo");
            ViewData["hash"] = hash.Hash;
            ViewData["salt"] = hash.Salt;


            var account = _buttery.Load(username);

            return View(new IndexModel() {
                Account = account,
            });
        }
    }
}