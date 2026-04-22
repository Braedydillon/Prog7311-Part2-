namespace Prog7311_Part2.Models
{

    // This matches the structure of most currency APIs
    public class ExchangeRateResponse
    {
        
        public decimal conversion_rate { get; set; }   

        public string? result { get; set; } 
        public double conversion_result { get; set; }

    }
}