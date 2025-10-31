using DragoTactical.Models;        // imports

namespace DragoTactical.Services;

//------------------------------------------------------------------------------------------------------
// Contact Service Interface - Defines contract for form submission processing

public interface IContactService
{
    //------------------------------------------------------------------------------------------------------
    // Process form submission
    Task<ContactSubmissionResult> ProcessSubmissionAsync(FormSubmission model, CancellationToken ct = default);
}

//------------------------------------------------------------------------------------------------------
// Contact submission result model
public sealed class ContactSubmissionResult
{
    public bool Success { get; init; }
    public string? Error { get; init; }
    public int SubmissionId { get; init; }
}
//-------------------------------------------------<<< Endof File >>>----------------------------------------------------