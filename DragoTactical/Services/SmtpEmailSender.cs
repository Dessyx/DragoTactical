using System.Net;
using System.Net.Mail;

namespace DragoTactical.Services
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly IConfiguration _config;
        private readonly ILogger<SmtpEmailSender> _logger;
        public SmtpEmailSender(IConfiguration config, ILogger<SmtpEmailSender> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SendAsync(string fromEmail, string subject, string body, CancellationToken ct = default)
        {
            var section = _config.GetSection("Smtp");
            var host = section.GetValue<string>("Host") ?? throw new InvalidOperationException("SMTP Host missing");
            var port = section.GetValue<int?>("Port") ?? 587;
            var user = section.GetValue<string>("User") ?? throw new InvalidOperationException("SMTP User missing");
            var pass = section.GetValue<string>("Password") ?? throw new InvalidOperationException("SMTP Password missing");
            var to = section.GetValue<string>("To") ?? throw new InvalidOperationException("SMTP To missing");
            var enableSsl = section.GetValue<bool?>("EnableSsl") ?? true;

            static string CleanHeader(string v) =>
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
                _logger.LogWarning("Invalid reply-to email: {Email}", fromEmail);
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
