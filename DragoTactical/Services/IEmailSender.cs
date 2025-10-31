using System.Threading;          // imports
using System.Threading.Tasks;

namespace DragoTactical.Services
{
    //------------------------------------------------------------------------------------------------------
    // Email Sender Interface - Defines contract for sending emails
    public interface IEmailSender
    {
        //------------------------------------------------------------------------------------------------------
        // Send email message
        Task SendAsync(string fromEmail, string subject, string body, CancellationToken ct = default);
    }
}
//-------------------------------------------------<<< Endof File >>>----------------------------------------------------
