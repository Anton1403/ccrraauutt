using crraut.Models;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using System.Diagnostics;
using System.Text.Json;

namespace crraut.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger) {
            _logger = logger;
        }

        public IActionResult Index() {
            return View();
        }

        public async Task<IActionResult> Privacy() {
            var client = new RestClient("https://api.ipify.org?format=json");
            var request = new RestRequest();
            var response = await client.GetAsync<IpResponse>(request);

            ViewData["IpAddress"] = response.Ip;

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    public class IpResponse
    {
        public string Ip { get; set; }
    }
}
