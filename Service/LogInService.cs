using Newtonsoft.Json;
using System.Text;
using VuelingExchangeManagerClient.Models;
using VuelingExchangeManagerClient.RequestDtos;

namespace VuelingExchangeManagerClient.Service
{
    public class LogInService
    {
        private readonly HttpClient _client;


        public LogInService(HttpClient client)
        {
            _client = client;

        }

        public async Task<string> GetJwtToken(string email, string password)
        {
            var endpoint = $"User/Login?givenEmail={Uri.EscapeDataString(email)}&givenPassword={Uri.EscapeDataString(password)}";
            var request = new HttpRequestMessage(HttpMethod.Post, endpoint);

            var response = await _client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var jwtToken = JsonConvert.DeserializeObject<dynamic>(responseBody).token;
                return jwtToken;
            }

            return null;
        }

        public async Task<bool> CreateCustomer(CustomerRequestDto customer)
        {
            var json = JsonConvert.SerializeObject(customer);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("User/CreateCustomer", data);

            return response.IsSuccessStatusCode;
        }

    }
}
