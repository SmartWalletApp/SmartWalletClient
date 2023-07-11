using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using VuelingExchangeManagerClient.Models;
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
        public async Task<IActionResult> SignIn(Customer customer, string repass)
        {
            // Comprobando que las contraseñas coinciden
            if (customer.Password != repass)
            {
                ModelState.AddModelError("repass", "Las contraseñas no coinciden");
                return View(customer);
            }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5233/");

                var json = JsonConvert.SerializeObject(customer);
                var data = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("User/CreateCustomer", data);


                if (response.IsSuccessStatusCode)
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
}
