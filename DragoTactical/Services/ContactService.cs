using DragoTactical.Models;        // imports
using Microsoft.EntityFrameworkCore;

namespace DragoTactical.Services;

//------------------------------------------------------------------------------------------------------
// Contact Service - Processes form submissions with encryption and email notifications

public sealed class ContactService : IContactService
{
    private readonly DragoTacticalDbContext _db;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<ContactService> _logger;

    //------------------------------------------------------------------------------------------------------
    // Constructor
    public ContactService(DragoTacticalDbContext db, IEmailSender emailSender, ILogger<ContactService> logger)
    {
        _db = db;
        _emailSender = emailSender;
        _logger = logger;
    }

    //------------------------------------------------------------------------------------------------------
    // Process form submission with encryption and email notification
    public async Task<ContactSubmissionResult> ProcessSubmissionAsync(FormSubmission model, CancellationToken ct = default)
    {
        if (model == null) return new ContactSubmissionResult { Success = false, Error = "Invalid form data." };

        var plainEmail = model.Email;
        model.FirstName = FieldEncryption.EncryptString(model.FirstName) ?? model.FirstName;
        model.LastName = FieldEncryption.EncryptString(model.LastName) ?? model.LastName;
        model.Email = FieldEncryption.EncryptString(model.Email) ?? model.Email;
        model.PhoneNumber = FieldEncryption.EncryptString(model.PhoneNumber) ?? model.PhoneNumber;
        model.CompanyName = FieldEncryption.EncryptString(model.CompanyName) ?? model.CompanyName;
        model.Location = FieldEncryption.EncryptString(model.Location) ?? model.Location;
        model.Message = FieldEncryption.EncryptString(model.Message) ?? model.Message;
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
                $"I am {FieldEncryption.DecryptString(model.FirstName)} {FieldEncryption.DecryptString(model.LastName)} situated in {FieldEncryption.DecryptString(model.Location)}.\nI need help with {serviceName}. " +
                $"\nMessage description: {FieldEncryption.DecryptString(model.Message)}\n\nContact details:\nCompany name:{FieldEncryption.DecryptString(model.CompanyName)}\nEmail:{FieldEncryption.DecryptString(model.Email)}\nPhone: {FieldEncryption.DecryptString(model.PhoneNumber)}\n";

            await _emailSender.SendAsync(plainEmail, subject, body, ct);

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
//-------------------------------------------------<<< Endof File >>>----------------------------------------------------