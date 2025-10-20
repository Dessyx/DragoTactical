using DragoTactical.Models;
using Microsoft.EntityFrameworkCore;

namespace DragoTactical.Services;

public sealed class ContactService : IContactService
{
    private readonly DragoTacticalDbContext _db;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<ContactService> _logger;

    public ContactService(DragoTacticalDbContext db, IEmailSender emailSender, ILogger<ContactService> logger)
    {
        _db = db;
        _emailSender = emailSender;
        _logger = logger;
    }

    public async Task<ContactSubmissionResult> ProcessSubmissionAsync(FormSubmission model, CancellationToken ct = default)
    {
        if (model == null) return new ContactSubmissionResult { Success = false, Error = "Invalid form data." };

       
        if (model.ServiceId == 0) model.ServiceId = null;
        model.SubmissionDate = DateTime.UtcNow;

        try
        {
            _db.FormSubmissions.Add(model);
            var changes = await _db.SaveChangesAsync(ct);
            if (changes == 0)
            {
                _logger.LogError("SaveChanges returned 0 - no rows affected");
                return new ContactSubmissionResult { Success = false, Error = "Form submission failed to save." };
            }

            string serviceName = "(Other / Not specified)";
            if (model.ServiceId.HasValue)
            {
                var name = await _db.Services
                    .AsNoTracking()
                    .Where(s => s.ServiceId == model.ServiceId.Value)
                    .Select(s => s.ServiceName)
                    .FirstOrDefaultAsync(ct);

                if (!string.IsNullOrWhiteSpace(name))
                    serviceName = name!;
            }

          
            var subject = serviceName;
            var body =
                $"I am {model.FirstName} {model.LastName} situated in {model.Location}.\nI need help with {serviceName}. " +
                $"\nMessage description: {model.Message}\n\nContact details:\nComapny name:{model.CompanyName}\nEmail:{model.Email}\nPhone: {model.PhoneNumber}\n";

           
            await _emailSender.SendAsync(model.Email, subject, body, ct);

            _logger.LogInformation("Form submission stored (Id={Id}) and email sent", model.SubmissionId);
            return new ContactSubmissionResult { Success = true, SubmissionId = model.SubmissionId };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while processing form submission");
            return new ContactSubmissionResult { Success = false, Error = "An error occurred while processing your submission." };
        }
    }
}