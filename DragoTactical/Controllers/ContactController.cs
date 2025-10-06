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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(FormSubmission model)
        {
            try
            {
                _logger.LogInformation("=== FORM SUBMISSION STARTED ===");
                _logger.LogInformation("Request Method: {Method}", Request.Method);
                _logger.LogInformation("Content Type: {ContentType}", Request.ContentType);
                _logger.LogInformation("Form Keys: {Keys}", string.Join(", ", Request.Form.Keys));

                if (model == null)
                {
                    _logger.LogWarning("Form submission received with null model");
                    TempData["ErrorMessage"] = "Invalid form data received - model is null.";
                    return Redirect(Request.Headers["Referer"].ToString() ?? "/");
                }

                _logger.LogInformation("Model received - FirstName: '{FirstName}', LastName: '{LastName}', Email: '{Email}', PhoneNumber: '{PhoneNumber}', CompanyName: '{CompanyName}', Location: '{Location}', ServiceId: {ServiceId}, Message: '{Message}'",
                    model.FirstName, model.LastName, model.Email, model.PhoneNumber, model.CompanyName, model.Location, model.ServiceId, model.Message);

                _logger.LogInformation("Raw form data - Keys: {Keys}", string.Join(", ", Request.Form.Keys));
                foreach (var key in Request.Form.Keys)
                {
                    _logger.LogInformation("Form[{Key}] = {Value}", key, Request.Form[key]);
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("=== MODEL STATE ERRORS ===");
                    foreach (var kvp in ModelState)
                    {
                        if (kvp.Value.Errors.Any())
                        {
                            _logger.LogWarning("Field '{Field}': {Errors}", kvp.Key, string.Join("; ", kvp.Value.Errors.Select(e => e.ErrorMessage)));
                        }
                    }
                }

                if (ModelState.IsValid)
                {
                    _logger.LogInformation("Model validation passed, attempting to save to database...");

                    model.SubmissionDate = DateTime.UtcNow;
                    _logger.LogInformation("Submission date set to: {Date}", model.SubmissionDate);

                    // Tests database connection first
                    _logger.LogInformation("Testing database connection...");
                    var canConnect = await _dbContext.Database.CanConnectAsync();
                    _logger.LogInformation("Database connection test result: {CanConnect}", canConnect);

                    if (!canConnect)
                    {
                        _logger.LogError("Cannot connect to database!");
                        TempData["ErrorMessage"] = "Database connection failed. Please check the SQLite database file path.";
                        return Redirect(Request.Headers["Referer"].ToString() ?? "/");
                    }

            
                    // Checks current count before adding
                    var currentCount = await _dbContext.FormSubmissions.CountAsync();
                    _logger.LogInformation("Current FormSubmissions count before adding: {Count}", currentCount);

                    // Adds to database
                    _logger.LogInformation("Adding form submission to database...");
                    _dbContext.FormSubmissions.Add(model);

                    _logger.LogInformation("Saving changes to database...");
                    var changesSaved = await _dbContext.SaveChangesAsync();
                    _logger.LogInformation("Changes saved: {ChangesCount} records affected", changesSaved);

                    // Verifies the record was actually added
                    var newCount = await _dbContext.FormSubmissions.CountAsync();
                    _logger.LogInformation("FormSubmissions count after adding: {Count}", newCount);

                    if (newCount > currentCount)
                    {
                        _logger.LogInformation("Form submission successfully saved from {Email} for service {ServiceId}",
                            model.Email, model.ServiceId);
                    }
                    else
                    {
                        _logger.LogError("Form submission was not saved - count did not increase!");
                        TempData["ErrorMessage"] = "Form submission was not saved to database.";
                        return Redirect(Request.Headers["Referer"].ToString() ?? "/");
                    }

                    // Redirect to success page or back to referring page
                    TempData["SuccessMessage"] = "Thank you for your submission! We'll get back to you soon.";
                    return Redirect(Request.Headers["Referer"].ToString() ?? "/");
                }
                else
                {
                    _logger.LogWarning("=== VALIDATION FAILED ===");
                    var errorMessages = new List<string>();
                    foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        errorMessages.Add(error.ErrorMessage);
                        _logger.LogWarning("Validation error: {Error}", error.ErrorMessage);
                    }

                    TempData["ErrorMessage"] = $"Please correct the errors and try again: {string.Join(", ", errorMessages)}";
                    return Redirect(Request.Headers["Referer"].ToString() ?? "/");
                }
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database update error saving form submission from {Email}", model?.Email ?? "unknown");
                TempData["ErrorMessage"] = $"Database error: {dbEx.InnerException?.Message ?? dbEx.Message}";
                return Redirect(Request.Headers["Referer"].ToString() ?? "/");
            }
            catch (InvalidOperationException ioEx)
            {
                _logger.LogError(ioEx, "Invalid operation error saving form submission from {Email}", model?.Email ?? "unknown");
                TempData["ErrorMessage"] = $"Invalid operation: {ioEx.Message}";
                return Redirect(Request.Headers["Referer"].ToString() ?? "/");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error saving form submission from {Email}", model?.Email ?? "unknown");
                TempData["ErrorMessage"] = $"Unexpected error: {ex.GetType().Name} - {ex.Message}";
                return Redirect(Request.Headers["Referer"].ToString() ?? "/");
            }
            finally
            {
                _logger.LogInformation("=== FORM SUBMISSION ENDED ===");
            }
        }
    }
}
