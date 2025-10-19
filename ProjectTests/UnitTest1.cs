using DragoTactical.Controllers;
using DragoTactical.Models;
using DragoTactical.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;

namespace ProjectTests
{
    public class ContactFormTests
    {
        private readonly Mock<IContactService> _contactServiceMock;
        private readonly Mock<ILogger<ContactController>> _loggerMock;

        public ContactFormTests()
        {
            _contactServiceMock = new Mock<IContactService>();
            _loggerMock = new Mock<ILogger<ContactController>>();
        }

        private ContactController GetController()
        {
            var controller = new ContactController(_loggerMock.Object, _contactServiceMock.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Scheme = "http";
            httpContext.Request.Host = new HostString("localhost");
            httpContext.Request.Headers["Referer"] = "http://localhost/";

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            controller.TempData = new TempDataDictionary(
                httpContext,
                Mock.Of<ITempDataProvider>());

            return controller;
        }

        private static FormSubmission ValidForm(int? serviceId = null)
        {
            return new FormSubmission
            {
                FirstName = "Cara",
                LastName = "Van",
                Email = "c.van@example.com",
                PhoneNumber = "044 884 1012 ",
                CompanyName = "HSec",
                Location = "South Africa",
                ServiceId = serviceId,
                Message = "I would like to inquire about services that are suitable for my company.",
            };
        }

        [Fact]
        public async Task Submit_SubmissionSuccess()
        {
            // Arrange
            var controller = GetController();
            var form = ValidForm();

            _contactServiceMock
                .Setup(s => s.ProcessSubmissionAsync(It.IsAny<FormSubmission>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ContactSubmissionResult { Success = true, Error = null });

            // Act
            var result = await controller.Submit(form, default);

            // Assert
            var redirect = Assert.IsType<RedirectResult>(result);
            Assert.Equal("/", redirect.Url);
            Assert.Equal("Thank you for your submission! We'll get back to you soon.", controller.TempData["SuccessMessage"]);         
        }

        [Fact]
        public async Task Submit_SubmissionFailure()
        {
            // Arrange
            var controller = GetController();
            var form = ValidForm();

            _contactServiceMock
                .Setup(s => s.ProcessSubmissionAsync(It.IsAny<FormSubmission>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ContactSubmissionResult { Success = false, Error = "Service error" });

            // Act
            var result = await controller.Submit(form, default);

            // Assert
            var redirect = Assert.IsType<RedirectResult>(result);
            Assert.Equal("/", redirect.Url);
            Assert.Equal("Service error", controller.TempData["ErrorMessage"]);           
        }
    }
}