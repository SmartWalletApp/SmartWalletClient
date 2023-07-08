using Newtonsoft.Json;
using System.Text;

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
            var httpClient = new HttpClient();

            // Configura los detalles de la solicitud
            var url = new Uri($"http://localhost:5233/Endpoint/Login?givenEmail={Uri.EscapeDataString(email)}&givenPassword={Uri.EscapeDataString(password)}");
            var request = new HttpRequestMessage(HttpMethod.Post, url);

            // Envía la solicitud y obtén la respuesta
            var response = await httpClient.SendAsync(request);

            // Si la respuesta fue exitosa, extrae el token del cuerpo de la respuesta
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var jwtToken = JsonConvert.DeserializeObject<dynamic>(responseBody).token;
                return jwtToken;
            }

            return null;
        }


    }
}
