using Microsoft.AspNetCore.Mvc;
using DragoTactical.Models;
using Microsoft.EntityFrameworkCore;

namespace DragoTactical.Controllers
{
    public class ContactController : Controller
    {
        private readonly DragoTacticalDbContext _dbContext;
        private readonly ILogger<ContactController> _logger;

        public ContactController(DragoTacticalDbContext dbContext, ILogger<ContactController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        private string GetReturnUrl()
        {
            var referer = Request.Headers["Referer"].ToString();
            return string.IsNullOrWhiteSpace(referer) ? "/" : referer;
        }

        private void LogModelStateErrors()
        {
            if (ModelState.IsValid) return;
            _logger.LogWarning("=== MODEL STATE ERRORS ===");
            foreach (var kvp in ModelState)
            {
                var entry = kvp.Value;
                if (entry?.Errors != null && entry.Errors.Count > 0)
                {
                    _logger.LogWarning("Field '{Field}': {Errors}", kvp.Key, string.Join("; ", entry.Errors.Select(e => e.ErrorMessage)));
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> TestConnection()
        {
            try
            {
                var canConnect = await _dbContext.Database.CanConnectAsync();
                var formCount = await _dbContext.FormSubmissions.CountAsync();
                return Json(new { canConnect, formCount, message = "Database connection test successful" });
            }
            catch (Exception ex)
            {
                return Json(new { canConnect = false, formCount = 0, error = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(FormSubmission model)
        {
            const string successMsg = "Thank you for your submission! We'll get back to you soon.";
            var returnUrl = GetReturnUrl();
            _logger.LogInformation("=== FORM SUBMISSION STARTED ===");

            if (model == null)
            {
                _logger.LogWarning("Null model received");
                TempData["ErrorMessage"] = "Invalid form data received.";
                return Redirect(returnUrl);
            }

            _logger.LogInformation("Incoming model: FirstName='{FirstName}', LastName='{LastName}', Email='{Email}', Phone='{Phone}', Company='{Company}', Location='{Location}', ServiceId={ServiceId}",
                model.FirstName, model.LastName, model.Email, model.PhoneNumber, model.CompanyName, model.Location, model.ServiceId);

            if (!ModelState.IsValid)
            {
                LogModelStateErrors();
                TempData["ErrorMessage"] = "Please correct the errors and try again.";
                return Redirect(returnUrl);
            }

            if (model.ServiceId == 0)
            {
                model.ServiceId = null;
            }

            model.SubmissionDate = DateTime.UtcNow;

            try
            {
                _dbContext.FormSubmissions.Add(model);
                var changes = await _dbContext.SaveChangesAsync();
                if (changes == 0)
                {
                    _logger.LogError("SaveChanges returned 0 - no rows affected");
                    TempData["ErrorMessage"] = "Form submission failed to save.";
                    return Redirect(returnUrl);
                }

                TempData["SuccessMessage"] = successMsg;
                _logger.LogInformation("Form submission stored (Id={Id})", model.SubmissionId);
                return Redirect(returnUrl);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database update error");
                TempData["ErrorMessage"] = $"Database error: {dbEx.InnerException?.Message ?? dbEx.Message}";
                return Redirect(returnUrl);
            }
            catch (InvalidOperationException ioEx)
            {
                _logger.LogError(ioEx, "Invalid operation during save");
                TempData["ErrorMessage"] = $"Invalid operation: {ioEx.Message}";
                return Redirect(returnUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during form submission");
                TempData["ErrorMessage"] = $"Unexpected error: {ex.GetType().Name} - {ex.Message}";
                return Redirect(returnUrl);
            }
            finally
            {
                _logger.LogInformation("=== FORM SUBMISSION ENDED ===");
            }
        }
    }
}
