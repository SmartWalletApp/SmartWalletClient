using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Text;
using VuelingExchangeManagerClient.Models;
using VuelingExchangeManagerClient.RequestDtos;
using VuelingExchangeManagerClient.ResponseDtos;

namespace VuelingExchangeManagerClient.Service
{
    public class CustomerService
    {
        private readonly HttpClient _client;


        public CustomerService(HttpClient client)
        {
            _client = client;

        }

        public async Task<CustomerResponseDto> GetMyCustomerAsync(string jwtToken)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(jwtToken);


            HttpResponseMessage response = await _client.GetAsync($"User/GetMyCustomer");
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<CustomerResponseDto>(content);
            }
            else
            {
                throw new Exception("No se pudo obtener el customer");
            }
        }
        public async Task<bool> AddBalance(BalanceHistoryRequestDto balanceData, string coinName, string jwtToken)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(jwtToken);

            string url = $"User/AddHistoric/{coinName}";

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

        public async Task<string> ViewBalance(string coinName, string jwtToken, DateTime minDate, DateTime maxDate)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(jwtToken);

            HttpResponseMessage response;

            response = await _client.GetAsync(
                $"User/" +
                $"GetBalanceHistorics/" +
                $"{System.Web.HttpUtility.UrlEncode(coinName)}" +
                $"?since={System.Web.HttpUtility.UrlEncode(minDate.ToString("yyyy/MM/dd"))}" +
                $"&until={System.Web.HttpUtility.UrlEncode(maxDate.AddDays(1).ToString("yyyy/MM/dd"))}"
                );


            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                return json;
            }
            else
            {
                throw new Exception("Error al obtener el balance");
            }
        }

        public async Task<bool> AddWallet(WalletRequestDto walletData, string coinName, string jwtToken)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(jwtToken);

            string url = $"User/AddWallet/{coinName}";

            var jsonContent = new StringContent(JsonConvert.SerializeObject(walletData), Encoding.UTF8, "application/json");

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
