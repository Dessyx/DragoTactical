using Microsoft.AspNetCore.Mvc;
using DragoTactical.Models;
using DragoTactical.Services;
using Microsoft.AspNetCore.RateLimiting;

namespace DragoTactical.Controllers
{
    public class ContactController : Controller
    {
        private readonly ILogger<ContactController> _logger;
        private readonly IContactService _contactService;

        public ContactController(ILogger<ContactController> logger, IContactService contactService)
        {
            _logger = logger;
            _contactService = contactService;
        }
        
        private string GetReturnUrl()
        {
            var referer = Request.Headers["Referer"].ToString();
            // Always default to home if referer is empty
            if (string.IsNullOrWhiteSpace(referer))
                return Url.Content("~/");

            if (Uri.TryCreate(referer, UriKind.Absolute, out var uri))
            {
                var currentHost = Request.Host.Host;
                if (uri.Host.Equals(currentHost, StringComparison.OrdinalIgnoreCase))
                {
                    var localPath = uri.PathAndQuery;
                    if (Url.IsLocalUrl(localPath))
                        return localPath;
                }
            return Url.Content("~/");
            }
            return Url.IsLocalUrl(referer) ? referer : Url.Content("~/");
        }

        private void LogModelStateErrors()
        {
            if (ModelState.IsValid) return;
            _logger.LogWarning("Model State errors");
            foreach (var kvp in ModelState)
            {
                var entry = kvp.Value;
                if (entry?.Errors != null && entry.Errors.Count > 0)
                {
                    _logger.LogWarning("Field '{Field}': {Errors}", kvp.Key, string.Join("; ", entry.Errors.Select(e => e.ErrorMessage)));
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EnableRateLimiting("FormSubmissions")]
        public async Task<IActionResult> Submit(FormSubmission model, CancellationToken ct)
        {
            const string successMsg = "Thank you for your submission! We'll get back to you soon.";
            var returnUrl = GetReturnUrl();
            _logger.LogInformation("Form Submission Started");

            if (model == null)
            {
                TempData["ErrorMessage"] = "Invalid form data received.";
                return Redirect(returnUrl);
            }

            if (!ModelState.IsValid)
            {
                LogModelStateErrors();
                TempData["ErrorMessage"] = "Please correct the errors and try again.";
                return Redirect(returnUrl);
            }

            var result = await _contactService.ProcessSubmissionAsync(model, ct);

            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Error ?? "An error occurred while processing your submission.";
                return Redirect(returnUrl);
            }

            TempData["SuccessMessage"] = successMsg;
            _logger.LogInformation("Form Submission Ended");
            return Redirect(returnUrl);
        }


    }
}
