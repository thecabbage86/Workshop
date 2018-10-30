
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Fortune_Teller_UI.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;

namespace Fortune_Teller_UI.Controllers
{
    public class FortunesController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ILogger<FortunesController> _logger;
        private readonly IFortuneService _fortuneService;
        private readonly IConfiguration _configuration;

        public FortunesController(ILogger<FortunesController> logger, IFortuneService fortuneService, IHostingEnvironment hostingEnvironment, IConfiguration configuration)
        {
            _logger = logger;
            _fortuneService = fortuneService;
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            _logger?.LogDebug("Index");

            string fortune = HttpContext.Session.GetString("MyFortune");
            ViewData["MyFortune"] = !String.IsNullOrEmpty(fortune) ? fortune : "DEFAULT FORTUNE";
            ViewData["Environment"] = _hostingEnvironment.EnvironmentName;
            ViewData["ServiceAddress"] = _configuration.GetSection("fortuneService")?.Get<FortuneServiceOptions>()?.Address;
            return View();
        }

        public async Task<IActionResult> RandomFortune()
        {
            _logger?.LogDebug("RandomFortune");

            var fortune = await _fortuneService.RandomFortuneAsync();

            HttpContext.Session.SetString("MyFortune", fortune.Text); 
            return View(fortune);
        }

        [HttpPost]
        public async Task<IActionResult> LogOff()
        {
            await HttpContext.SignOutAsync();
            HttpContext.Session.Clear();
            await HttpContext.Session.CommitAsync();
            return RedirectToAction(nameof(FortunesController.Index), "Fortunes");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return RedirectToAction(nameof(FortunesController.Index), "Fortunes");
        }

        public IActionResult Manage()
        {
            ViewData["Message"] = "Manage accounts using UAA or CF command line.";
            return View();
        }

        public IActionResult AccessDenied()
        {
            ViewData["Message"] = "Insufficient permissions.";
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

    }
}
