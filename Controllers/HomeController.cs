using Microsoft.AspNetCore.Mvc;
using butterystrava.Models;
using System.Linq;
using butterystrava.Strava;

namespace butterystrava.Controllers {

    public class HomeController : Controller {

        private readonly Settings _settings;
        private readonly Buttery.Buttery _buttery;
        private readonly Account _account;

        public HomeController(ButteryContext context, Strava.Settings settings) {
            // todo move database into another controller. keep home static
            _settings = settings;

            _buttery = new Buttery.Buttery(context);
            _account = _buttery.GetAccount();
        }

        public IActionResult Index() {
            return View(new IndexModel() {
                ClientId = _settings.ClientId,
                Account = _account
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