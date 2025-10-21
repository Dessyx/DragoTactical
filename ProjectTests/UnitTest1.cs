using DragoTactical.Controllers;
using DragoTactical.Models;
using DragoTactical.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Moq;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace ProjectTests
{
    public class ContactFormTests
    {
        private readonly Mock<IContactService> _contactServiceMock;
        private readonly Mock<ILogger<ContactController>> _loggerMock;
        private readonly WebApplicationFactory<Program> _factory;

        public ContactFormTests()
        {
            _contactServiceMock = new Mock<IContactService>();
            _loggerMock = new Mock<ILogger<ContactController>>();
            _factory = new WebApplicationFactory<Program>();
        }

        private List<ValidationResult> ValidateModel(FormSubmission model)
        {
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(model);
            Validator.TryValidateObject(model, context, validationResults, true);
            return validationResults;
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
            var controller = GetController();
            var form = ValidForm();

            _contactServiceMock
                .Setup(s => s.ProcessSubmissionAsync(It.IsAny<FormSubmission>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ContactSubmissionResult { Success = false, Error = "Service error" });

            
            var result = await controller.Submit(form, default);

            
            var redirect = Assert.IsType<RedirectResult>(result);
            Assert.Equal("/", redirect.Url);
            Assert.Equal("Service error", controller.TempData["ErrorMessage"]);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Validate_MissingEmail(string email)
        {
            
            var form = new FormSubmission
            {
                FirstName = "Cara",
                LastName = "Van",
                Email = email
            };

            
            var errors = ValidateModel(form);

            
            Assert.NotEmpty(errors);
            Assert.Contains(errors, e => e.MemberNames.Contains("Email"));
        }

        [Theory]
        [InlineData("invalid-email")]
        [InlineData("@example.com")]
        [InlineData("user@")]
        [InlineData("user@@example.com")]
        public void Validate_InvalidEmail(string email)
        {
            
            var form = new FormSubmission
            {
                FirstName = "Cara",
                LastName = "Van",
                Email = email
            };

            
            var errors = ValidateModel(form);

            
            Assert.NotEmpty(errors);
            Assert.Contains(errors, e => e.MemberNames.Contains("Email") &&
                                         e.ErrorMessage == "Invalid email address");
        }

        [Theory]
        [InlineData("user@example.com")]
        [InlineData("user.name@example.co.za")]
        [InlineData("user+tag@example.com")]
        [InlineData("user_name@example-domain.com")]
        public void Validate_ValidEmail(string email)
        {
            
            var form = new FormSubmission
            {
                FirstName = "Cara",
                LastName = "Van",
                Email = email
            };

            
            var errors = ValidateModel(form);

            
            Assert.Empty(errors);
        }

        [Fact]
        public void Validate_EmailTooLong()
        {
            var form = new FormSubmission
            {
                FirstName = "Cara",
                LastName = "Van",
                Email = new string('a', 250) + "@example.com" // Over 255 chars
            };

            var errors = ValidateModel(form);

            Assert.NotEmpty(errors);
            Assert.Contains(errors, e => e.MemberNames.Contains("Email") &&
                                         e.ErrorMessage.Contains("cannot exceed 255 characters"));
        }

        [Fact]
        public void Validate_MultipleErrors()
        {
            var form = new FormSubmission
            {
                FirstName = "",
                LastName = "",
                Email = "invalid-email",
                PhoneNumber = "abc",
                CompanyName = "<script>alert('xss')</script>"
            };

            var errors = ValidateModel(form);

            // Assert - tests for multiple validation errors
            Assert.NotEmpty(errors);
            Assert.True(errors.Count >= 5);
            Assert.Contains(errors, e => e.MemberNames.Contains("FirstName"));
            Assert.Contains(errors, e => e.MemberNames.Contains("LastName"));
            Assert.Contains(errors, e => e.MemberNames.Contains("Email"));
            Assert.Contains(errors, e => e.MemberNames.Contains("PhoneNumber"));
            Assert.Contains(errors, e => e.MemberNames.Contains("CompanyName"));
        }

    }
}