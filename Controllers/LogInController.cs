using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using VuelingExchangeManagerClient.Models;
using VuelingExchangeManagerClient.RequestDtos;
using VuelingExchangeManagerClient.Service;

namespace VuelingExchangeManagerClient.Controllers
{
    public class LogInController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly LogInService _loginService;


        public LogInController(ILogger<HomeController> logger, LogInService loginService)
        {
            _logger = logger;
            _loginService = loginService;
        }
        public IActionResult LogIn()
        {
            return View();
        }
        public IActionResult SignIn()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> LogIn(string email, string password)
        {
            var jwtToken = await _loginService.GetJwtToken(email, password);

            if (jwtToken == null)
            {
                return View();
            }

            HttpContext.Session.SetString("jwtToken", jwtToken);

            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public async Task<IActionResult> SignIn(CustomerRequestDto customer, string repass)
        {
            var isCustomerCreated = await _loginService.CreateCustomer(customer);

            if (isCustomerCreated)
            {
                return RedirectToAction("LogIn", "LogIn");
            }
            else
            {
                ModelState.AddModelError("", "No se pudo registrar el usuario");
                return View(customer);
            }
        }
    }
}
