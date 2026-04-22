using Xunit;
using Microsoft.AspNetCore.Http;
using System.IO;
using Moq;

namespace Prog7311_UnitTests
{
    public class FileValidationTests
    {
        [Theory]
        [InlineData("document.pdf", true)]
        [InlineData("virus.exe", false)]
        [InlineData("picture.png", false)]
        public void IsFileValid_ShouldOnlyAllowPdf(string fileName, bool expectedResult)
        {
            // --- ARRANGE ---
            // Fake an IFormFile
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(_ => _.FileName).Returns(fileName);

            // Logic: Check extension
            var extension = Path.GetExtension(fileMock.Object.FileName).ToLower();

            // --- ACT ---
            bool isValid = (extension == ".pdf");

            // --- ASSERT ---
            Assert.Equal(expectedResult, isValid);
        }
    }
}