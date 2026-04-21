namespace Prog7311_Part2.Models
{
    
        // This matches the structure of most currency APIs
        public class ExchangeRateResponse
        {
            public string result { get; set; }      // "success" or "error"
            public decimal conversion_result { get; set; } // The final converted amount
            public decimal conversion_rate { get; set; }   // The rate used (e.g. 18.50)
        }
    }