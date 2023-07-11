using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Text;
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

        public async Task<Customer> GetMyCustomerAsync(string jwtToken)
        {
           
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(jwtToken);


            HttpResponseMessage response = await _client.GetAsync("http://localhost:5233/User/GetMyCustomer");
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
        public async Task<bool> AddBalance(BalanceHistory balanceData, string coinName, string jwtToken)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(jwtToken);

            string url = $"http://localhost:5233/User/AddHistoric/{coinName}";

            var jsonContent = new StringContent(JsonConvert.SerializeObject(balanceData), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync(url, jsonContent);

            if (response.IsSuccessStatusCode)
            {
                // El balance se añadió con éxito
                return true;
            }
            else
            {
                // Hubo un error al añadir el balance
                return false;
            }
        }




    }


}
