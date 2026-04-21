using Prog7311_Part2.Models;
using System.Net.Http.Json; // Make sure this is here for GetFromJsonAsync

namespace Prog7311_Part2.Services
{
    public class CurrencyService
    {
        private readonly HttpClient _httpClient;

        public CurrencyService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<decimal> ConvertToZAR(decimal amount, string fromCurrency)
        {
            // Using the API URL structure that matches your ExchangeRateResponse model
            // Replace YOUR_API_KEY with your actual key if using exchangerate-api.com
            string url = $"https://v6.exchangerate-api.com/v6/43ee498784381b01875dbe0e/latest/USD{fromCurrency}/ZAR/{amount}";

            var response = await _httpClient.GetFromJsonAsync<ExchangeRateResponse>(url);

            // Logic: If the API says success, return the converted money value
            if (response != null && response.result == "success")
            {
                return response.conversion_result;
            }

            // Fallback: If the API fails, return the original amount so the app doesn't crash
            return amount;
        }
    }
}