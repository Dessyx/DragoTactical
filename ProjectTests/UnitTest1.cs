using DragoTactical.Controllers;
using DragoTactical.Models;
using DragoTactical.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ProjectTests
{
    public class ContactControllerTests
    {
        private static ContactController CreateController(
        Mock<ILogger<ContactController>> loggerMock = null,
        Mock<IContactService> contactServiceMock = null,
        IUrlHelper urlHelper = null,
        ITempDataDictionary tempData = null,
        string refererHeader = null,
        string host = "example.com")
        {
            loggerMock ??= new Mock<ILogger<ContactController>>();
            contactServiceMock ??= new Mock<IContactService>();

            var controller = new ContactController(loggerMock.Object, contactServiceMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        Request =
                {
                    Host = new HostString(host)
                }
                    }
                }
            };

            if (refererHeader != null)
                controller.HttpContext.Request.Headers.Referer = refererHeader;

            // give it a real Url-helper (or the one supplied by the caller)
            controller.Url = urlHelper ?? Mock.Of<IUrlHelper>(u =>
                                u.Content("~/") == "/" &&
                                u.IsLocalUrl(It.IsAny<string>()) == true);

            controller.TempData = tempData ?? new TempDataDictionary(controller.HttpContext,
                                        Mock.Of<ITempDataProvider>());

            return controller;
        }

        private static IUrlHelper FakeUrlHelper()
        {
            var mock = new Mock<IUrlHelper>();
            mock.Setup(u => u.Content("~/")).Returns("/");
            mock.Setup(u => u.IsLocalUrl(It.IsAny<string>())).Returns(true);
            return mock.Object;
        }

        [Fact]
        public async Task Submit_NullModel_Redirects()
        {
            // Arrange
            var logger = new Mock<ILogger<ContactController>>();
            var contactService = new Mock<IContactService>();

            var urlMock = new Mock<IUrlHelper>();
            urlMock.Setup(u => u.Content("~/")).Returns("/");

            var controller = CreateController(logger, contactService, urlMock.Object, refererHeader: string.Empty);

            // Act
            var result = await controller.Submit(null, CancellationToken.None);

            // Assert
            result.Should().BeOfType<RedirectResult>();
            var rr = (RedirectResult)result;
            rr.Url.Should().Be("/");

            controller.TempData.Should().ContainKey("ErrorMessage");
            controller.TempData["ErrorMessage"].Should().Be("Invalid form data received.");

            contactService.Verify(s => s.ProcessSubmissionAsync(It.IsAny<FormSubmission>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task InvalidModelState_LogsWarnings()
        {
            // Arrange
            var logger = new Mock<ILogger<ContactController>>();
            var contactService = new Mock<IContactService>();

            var urlMock = new Mock<IUrlHelper>();
            urlMock.Setup(u => u.IsLocalUrl(It.IsAny<string>())).Returns(true);

            var controller = CreateController(logger, contactService, FakeUrlHelper(), refererHeader: "/previous");

            controller.ModelState.AddModelError("FirstName", "First name is required");

            var model = new FormSubmission
            {
                FirstName = "",
                LastName = "L",
                Email = "a@b.com"
            };

            // Act
            var result = await controller.Submit(model, CancellationToken.None);

            // Assert 
            result.Should().BeOfType<RedirectResult>();
            ((RedirectResult)result).Url.Should().Be("/previous");

            controller.TempData.Should().ContainKey("ErrorMessage");
            controller.TempData["ErrorMessage"].Should().Be("Please correct the errors and try again.");

            logger.Verify(
                x => x.Log<It.IsAnyType>(
                    It.Is<LogLevel>(l => l == LogLevel.Warning),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) =>
                        v.ToString().Contains("Model State errors") ||
                        v.ToString().Contains("Field")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.AtLeastOnce);

            contactService.Verify(s => s.ProcessSubmissionAsync(It.IsAny<FormSubmission>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task ProcessingFailure_ErrorMessage()
        {
            // Arrange
            var logger = new Mock<ILogger<ContactController>>();
            var contactService = new Mock<IContactService>();
            contactService
                .Setup(s => s.ProcessSubmissionAsync(It.IsAny<FormSubmission>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ContactSubmissionResult { Success = false, Error = "Processing failed" });

            var urlMock = new Mock<IUrlHelper>();
            urlMock.Setup(u => u.IsLocalUrl(It.IsAny<string>())).Returns(true);

            var controller = CreateController(logger, contactService, FakeUrlHelper(), refererHeader: "/contact");

            var model = new FormSubmission
            {
                FirstName = "dfgh",
                LastName = "BHJD",
                Email = "ab@cd.com"
            };

            // Act
            var result = await controller.Submit(model, CancellationToken.None);

            // Assert
            result.Should().BeOfType<RedirectResult>();
            ((RedirectResult)result).Url.Should().Be("/contact");

            controller.TempData.Should().ContainKey("ErrorMessage");
            controller.TempData["ErrorMessage"].Should().Be("Processing failed");

            contactService.Verify(s => s.ProcessSubmissionAsync(It.Is<FormSubmission>(m => m == model), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ProcessingSuccess_SetsSuccessMessage()
        {
            // Arrange
            var logger = new Mock<ILogger<ContactController>>();
            var contactService = new Mock<IContactService>();
            contactService
                .Setup(s => s.ProcessSubmissionAsync(It.IsAny<FormSubmission>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ContactSubmissionResult { Success = true, SubmissionId = 123 });

            var urlMock = new Mock<IUrlHelper>();
            urlMock.Setup(u => u.IsLocalUrl(It.IsAny<string>())).Returns(true);

            var controller = CreateController(logger, contactService, FakeUrlHelper(), refererHeader: "/thanks");

            var model = new FormSubmission
            {
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com"
            };

            // Act
            var result = await controller.Submit(model, CancellationToken.None);

            result.Should().BeOfType<RedirectResult>();
            ((RedirectResult)result).Url.Should().Be("/thanks");

            controller.TempData.Should().ContainKey("SuccessMessage");
            controller.TempData["SuccessMessage"].Should().Be("Thank you for your submission! We'll get back to you soon.");

            logger.Verify(
                x => x.Log<It.IsAnyType>(
                    It.Is<LogLevel>(l => l == LogLevel.Information),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) =>
                        v.ToString().Contains("Form Submission")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.AtLeastOnce);

            contactService.Verify(s => s.ProcessSubmissionAsync(It.Is<FormSubmission>(m => m == model), It.IsAny<CancellationToken>()), Times.Once);
        }

        private List<ValidationResult> ValidateModel(FormSubmission model)
        {
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(model);
            Validator.TryValidateObject(model, context, validationResults, true);
            return validationResults;
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