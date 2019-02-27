using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace PokemonDatabase.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
        private readonly AppConfig _appConfig;

        public HomeController(IOptions<AppConfig> appConfig)
        {
            _appConfig = appConfig.Value;
        }

        [Route("")]
        public IActionResult Index()
        {
            ViewBag.ApplicationName = _appConfig.AppName;
            ViewBag.ApplicationVersion = _appConfig.AppVersion;

            return View();
        }
    }
}
