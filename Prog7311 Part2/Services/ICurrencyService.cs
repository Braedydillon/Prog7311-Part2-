namespace Prog7311_Part2.Services

{
    public interface ICurrencyService
    {
        
        Task<decimal> ConvertToZAR(decimal amount, string fromCurrency);
    }
}

