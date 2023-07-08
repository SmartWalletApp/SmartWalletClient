using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using VuelingExchangeManagerClient.Models;

namespace VuelingExchangeManagerClient.Service
{
    public class CustomerService
    {
        private readonly HttpClient _client;
        private readonly HttpClientHandler _handler;

        public CustomerService()
        {
            _handler = new HttpClientHandler();
            _handler.CookieContainer = new CookieContainer();

            _client = new HttpClient(_handler);
        }

        public async Task<Customer> GetCustomerByIdAsync(string jwtToken, int userId)
        {
            // Añade el token JWT como una cookie
            _handler.CookieContainer.Add(new Uri("http://localhost:5233/Endpoint"),
                                        new Cookie("jwt", jwtToken));

            HttpResponseMessage response = await _client.GetAsync($"http://localhost:5233/Endpoint/{userId}");
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Customer>(content);
            }
            else
            {
                throw new Exception("No se pudo obtener el customer");
            }
        }
    }


}
