using System.Net.Http.Json;

namespace Prog7311_Part2.Services
{
    // This implements your ICurrencyService interface
    public class CurrencyService : ICurrencyService
    {
        private readonly HttpClient _httpClient;

        public CurrencyService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<decimal> ConvertToZAR(decimal amount, string fromCurrency)
        {
            // If it's already ZAR, don't bother calling the API
            if (fromCurrency == "ZAR") return amount;

            try
            {
                // Replace this with your actual API key from ExchangeRate-API
                string apiKey = "YOUR_API_KEY_HERE"; 
                string url = $"https://v6.exchangerate-api.com/v6/43ee498784381b01875dbe0e/pair/{fromCurrency}/ZAR/{amount}";

                var response = await _httpClient.GetFromJsonAsync<ExchangeRateResponse>(url);

                if (response != null && response.conversion_result != 0)
                {
                    return (decimal)response.conversion_result;
                }
            }
            catch (Exception)
            {
                // Fallback: If API fails, return original amount so the app doesn't break
                return amount;
            }

            return amount;
        }
    }

    // Helper class to map the API response
    public class ExchangeRateResponse
    {
        public string result { get; set; }
        public double conversion_result { get; set; }
    }
}