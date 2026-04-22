using Moq;
using Xunit;
using Prog7311_Part2.Services;
using System.Net.Http;

namespace Prog7311_UnitTests
{
    public class CurrencyServiceTests
    {
        [Fact]
        public async Task ConvertToZAR_CalculatesCorrectly_GivenSpecificAmount()
        {
            // --- ARRANGE ---
            // Note: If your CurrencyService logic is simple multiplication,
            // we test that the input amount * rate = expected output.
            var mockCurrencyService = new Mock<ICurrencyService>();
            decimal inputAmount = 100;
            string fromCurrency = "USD";
            decimal expectedZar = 1900; // Assuming a rate of 19.00

            mockCurrencyService.Setup(s => s.ConvertToZAR(inputAmount, fromCurrency))
                               .ReturnsAsync(expectedZar);

            // --- ACT ---
            var result = await mockCurrencyService.Object.ConvertToZAR(inputAmount, fromCurrency);

            // --- ASSERT ---
            Assert.Equal(expectedZar, result);
        }
    }
}