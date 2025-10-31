using System.Net;                // imports
using System.Net.Mail;

namespace DragoTactical.Services
{
    //------------------------------------------------------------------------------------------------------
    // SMTP Email Sender - Sends emails via SMTP server
    public class SmtpEmailSender : IEmailSender
    {
        private readonly IConfiguration _config;
        private readonly ILogger<SmtpEmailSender> _logger;

        //------------------------------------------------------------------------------------------------------
        // Constructor
        public SmtpEmailSender(IConfiguration config, ILogger<SmtpEmailSender> logger)
        {
            _config = config;
            _logger = logger;
        }

        //------------------------------------------------------------------------------------------------------
        // Send email via SMTP
        public async Task SendAsync(string fromEmail, string subject, string body, CancellationToken ct = default)
        {
            var section = _config.GetSection("Smtp");
            var host = section.GetValue<string>("Host");
            if (string.IsNullOrEmpty(host))
            {
                throw new InvalidOperationException("SMTP Host missing");
            }
            var port = section.GetValue<int?>("Port") ?? 587;

            var user = section.GetValue<string>("User");
            if (string.IsNullOrEmpty(user))
            {
                throw new InvalidOperationException("SMTP User missing");
            }
            var pass = section.GetValue<string?>("Password");
            var to = section.GetValue<string?>("To");
            if (string.IsNullOrEmpty(to))
            {
                throw new InvalidOperationException("SMTP To address missing");
            }
            var enableSsl = section.GetValue<bool?>("EnableSsl") ?? true;

            static string CleanHeader(string v) =>
                string.IsNullOrEmpty(v) ? string.Empty : v.Replace("\r", "").Replace("\n", "").Trim();

            static string SanitizeForLogging(string v) =>
                string.IsNullOrEmpty(v) ? string.Empty : v.Replace("\r", "").Replace("\n", "").Trim();

            var safeSubject = CleanHeader(subject);
            if (safeSubject.Length > 200) safeSubject = safeSubject[..200];

            using var message = new MailMessage();
            message.From = new MailAddress(user, "DragoTactical Website");

            try
            {
                message.ReplyToList.Clear();
                message.ReplyToList.Add(new MailAddress(fromEmail));
            }
            catch
            {
                _logger.LogWarning("Invalid reply-to email: {Email}", SanitizeForLogging(fromEmail));
            }

            message.To.Add(new MailAddress(to));
            message.Subject = safeSubject;
            message.Body = body ?? string.Empty;
            message.IsBodyHtml = false;

            using var client = new SmtpClient(host, port)
            {
                EnableSsl = enableSsl,
                Credentials = new NetworkCredential(user, pass)
            };

            try
            {
                await Task.Run(() => client.Send(message), ct);
                _logger.LogInformation("Email sent to {To}", to);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email");
                throw;
            }
        }
    }
}
//-------------------------------------------------<<< Endof File >>>----------------------------------------------------
