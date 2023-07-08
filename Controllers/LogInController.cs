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
        public async Task<IActionResult> GetToken(string email, string password)
        {
            // Obtiene el token utilizando el método descrito anteriormente
            var jwtToken = await _loginService.GetJwtToken(email, password);

            // Si el token es nulo, eso significa que la autenticación falló
            if (jwtToken == null)
            {
                // Maneja el caso de autenticación fallida (puedes mostrar un mensaje de error, por ejemplo)
                return View();
            }

            // Almacena el token en una cookie
            HttpContext.Session.SetString("jwtToken", jwtToken);



            // Redirige al usuario a la página principal
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public async Task<IActionResult> Register(Customer customer, string repass)
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

                var response = await client.PostAsync("Endpoint", data);

                if (response.IsSuccessStatusCode)
                {
                    // Redirecciona a la página de éxito o realiza alguna otra acción
                    return RedirectToAction("LogIn", "LogIn");
                }
                else
                {
                    // Maneja el error
                    ModelState.AddModelError("", "No se pudo registrar el usuario");
                    return View(customer);
                }
            }
        }



    }
}
