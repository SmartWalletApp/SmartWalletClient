using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using SmartWalletClient.MVC.Models;
using SmartWalletClient.MVC.RequestDtos;
using SmartWalletClient.MVC.ResponseDtos;
using SmartWalletClient.MVC.Service;

namespace SmartWalletClient.MVC.Controllers
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

        [HttpPost]
        public IActionResult SelectedCoin(string coinName, string minDate, string maxDate)
        {
            HttpContext.Session.SetString("SelectedCoin", coinName);
            HttpContext.Session.SetString("SelectedCoinMinDate", minDate);
            HttpContext.Session.SetString("SelectedCoinMaxDate", maxDate);

            return RedirectToAction("Historics", "Home");
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

            var model = new Tuple<CustomerResponseDto>(customer);
            return View(model);

        }

        private string GetJwtToken()
        {
            return HttpContext.Session.GetString("jwtToken");
        }

        public async Task<IActionResult> Historics()
        {
            var jwtToken = GetJwtToken();

            var coinName = HttpContext.Session.GetString("SelectedCoin");
            var minDate = HttpContext.Session.GetString("SelectedCoinMinDate");
            var maxDate = HttpContext.Session.GetString("SelectedCoinMaxDate");

            DateTime minDateClean;
            DateTime maxDateClean;

            DateTime.TryParse(minDate, out minDateClean);
            DateTime.TryParse(maxDate, out maxDateClean);


            if (!string.IsNullOrEmpty(coinName))
            {
                var json = await _customerService.ViewBalance(coinName, jwtToken, minDateClean, maxDateClean);

                if (!string.IsNullOrEmpty(json))
                {
                    ViewData["JsonData"] = json;
                    return View();
                }
            }

            return View("Error");
        }




        [HttpPost]
        public async Task<IActionResult> AddWallet(WalletRequestDto walletData, string coinName)
        {
            var jwtToken = GetJwtToken();
            var success = await _customerService.AddWallet(walletData, coinName, jwtToken);

            if (success)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddBalance(BalanceHistoryRequestDto balanceData, string coinName)
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