using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
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

            // Obtiene el ID del usuario
            var userId = await GetUserId(jwtToken);
            if (userId == null)
            {
                // Maneja el error obteniendo el ID
                return View("Error");
            }

            // Pasa el ID a la vista
            ViewBag.UserId = userId;

            var customer = await _customerService.GetCustomerByIdAsync(jwtToken,(int)userId);
            var model = new Tuple<Customer>(customer);
            return View(model);

        }

        private string GetJwtToken()
        {
            return HttpContext.Session.GetString("jwtToken");
        }

        private async Task<int?> GetUserId(string jwt)
        {
            var handler = new JwtSecurityTokenHandler();

            // Verifica si el token puede ser leído
            if (handler.CanReadToken(jwt))
            {
                // Lee el token
                var token = handler.ReadJwtToken(jwt);

                // Obtiene el payload del token como un JSON
                string payloadJson = token.Payload.SerializeToJson();

                // Deserializa el JSON a un objeto
                var payload = JsonSerializer.Deserialize<Dictionary<string, object>>(payloadJson);

                // Extrae el "id" del payload
                if (payload.TryGetValue("id", out var idObj))
                {
                    // Intenta convertir el objeto ID a un string y luego a un int
                    if (int.TryParse(idObj.ToString(), out var id))
                    {
                        return id;
                    }
                    else
                    {
                        _logger.LogError("No se pudo convertir el ID a un número entero.");
                        return null;
                    }
                }
                else
                {
                    _logger.LogError("No se pudo extraer el ID del token JWT.");
                    return null;
                }
            }
            else
            {
                _logger.LogError("Error obteniendo el ID del usuario desde el Endpoint");
                return null;
            }
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}