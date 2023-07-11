using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Text.Json;
using VuelingExchangeManagerClient.Models;
using VuelingExchangeManagerClient.Service;

namespace VuelingExchangeManagerClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CustomerService _customerService;



        public HomeController(ILogger<HomeController> logger, CustomerService customerService)
        {
            _logger = logger;
            _customerService = customerService;
        }

        public async Task<IActionResult> Index()
        {
            var jwtToken = GetJwtToken();

            if (jwtToken == null)
            {
                return View("Error");
            }

            ViewBag.JwtToken = jwtToken;

            var customer = await _customerService.GetMyCustomerAsync(jwtToken);
            var model = new Tuple<Customer>(customer);
            return View(model);

        }

        private string GetJwtToken()
        {
            return HttpContext.Session.GetString("jwtToken");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddBalance(BalanceHistory balanceData, string coinName)
        {
            var jwtToken = GetJwtToken();
            var success = await _customerService.AddBalance(balanceData, coinName, jwtToken);

            if (success)
            {
                return RedirectToAction("Index", "Home"); 
            }
            else
            {
                return View("Error");  
            }
        }

        public IActionResult LogOut()
        {
            HttpContext.Session.Clear(); 

            return RedirectToAction("LogIn", "LogIn");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}