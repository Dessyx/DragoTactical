using System.Threading;
using System.Threading.Tasks;

namespace DragoTactical.Services
{
    public interface IEmailSender
    {
        Task SendAsync(string fromEmail, string subject, string body, CancellationToken ct = default);
    }
}
