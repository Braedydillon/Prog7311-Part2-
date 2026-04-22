using Moq;
using Xunit;
using Prog7311_Part2.Controllers;
using Prog7311_Part2.Repositories;
using Prog7311_Part2.Models;
using Prog7311_Part2.Services;
using Microsoft.AspNetCore.Mvc;

namespace Prog7311_UnitTests
{
    public class ServiceRequestTests
    {
        [Fact]
        public async Task Create_Fails_When_ContractIsBlocked()
        {
            // --- ARRANGE ---
            var mockRepo = new Mock<IServiceRequestRepository>();
            var mockContractRepo = new Mock<IContractRepository>();
            var mockCurrency = new Mock<ICurrencyService>();

            // Simulate the business rule: Contract 99 is BLOCKED (Expired)
            mockRepo.Setup(repo => repo.IsContractBlocked(99)).ReturnsAsync(true);

            var controller = new ServiceRequestsController(mockRepo.Object, mockContractRepo.Object, mockCurrency.Object);
            var invalidRequest = new ServiceRequest { ContractId = 99, Description = "Fix Server" };

            // --- ACT ---
            var result = await controller.Create(invalidRequest, "USD");

            // --- ASSERT ---
            // Verify it returns the View (doesn't redirect) and adds a ModelState error
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.False(controller.ModelState.IsValid);
        }
    }
}