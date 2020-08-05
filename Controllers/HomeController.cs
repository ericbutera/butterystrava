using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using System.Threading.Tasks;

using butterystrava.Buttery;
using butterystrava.Models;
using butterystrava.Strava;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace butterystrava.Controllers {

    public class HomeController : Controller {

        private readonly Settings _settings;
        private readonly Buttery.Buttery _buttery;
        private readonly Client _client;
        private readonly string _redirectUri;
        private readonly UserManager<User> _userManager;

        private Task<User> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }

        private string _username {
            get {
                return "TODO";//return HttpContext.Session.GetString("username");
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

        public HomeController(ButteryContext context, Strava.Settings settings, Strava.Client client, IConfiguration config,
        UserManager<User> userManager
        ) {
            _settings = settings;
            _buttery = new Buttery.Buttery(context);
            _client = client;
            _redirectUri = config["StravaOauthRedirectUri"];
            _userManager = userManager;
        }

        public async Task<RedirectToActionResult> Index() {
            var user = await GetCurrentUserAsync();
            if (user != null)
                return RedirectToAction("Welcome");

            return RedirectToAction("Login");
        }

        public async Task<IActionResult> Login()
        {
            var user = await GetCurrentUserAsync();
            if (user != null)
                return RedirectToAction("Welcome");

            ViewData["AuthorizationUrl"] = _client.GetAuthUrl(_redirectUri);
            return View();
        }

        public RedirectToActionResult Logout() 
        {
            _username = null;
            return RedirectToAction("Index");
        }

        /*public RedirectToActionResult Authorize(string username, string password) {
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
        }*/

        /*public RedirectToActionResult Password(string password) {
            var account = _buttery.Load(_username);

            var hash = HashLibrary.HashedPassword.New(password);
            account.Hash = hash.Hash;
            account.Salt = hash.Salt;

            _buttery.Save(account);

            return RedirectToAction("Welcome");
        }*/

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

        [Authorize]
        public async Task<IActionResult> Welcome() {
            var user = await GetCurrentUserAsync();
            if (user == null) //if (string.IsNullOrWhiteSpace(username))
                return RedirectToAction("Login");

            var username = user.AthleteUsername; // _username


            ViewData["username"] = username;
            var account = _buttery.Load(username);

            return View(new IndexModel() {
                Account = account,
            });
        }

                /// <summary>
        /// Instructs the middleware to redirect to the Strava OAuth 2.0 user login screen.
        /// After successful OAuth authentication the athletes profile response data is 
        /// added to the current user identity.
        /// </summary>
        public IActionResult Connect()
        {
            /// The authenticationSchemes parameter must be set to "Strava".
            return Challenge(new AuthenticationProperties { RedirectUri = "Strava/Connected" }, "Strava");
        }

        /// <summary>
        /// Strava login callback. 
        /// </summary>
        public IActionResult Connected()
        {
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Deletes the authentication cookie.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> SignOut()
        {
            await HttpContext.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }
    }
}