using DragoTactical.Models;

namespace DragoTactical.Services;

public interface IContactService
{
    Task<ContactSubmissionResult> ProcessSubmissionAsync(FormSubmission model, CancellationToken ct = default);
}

public sealed class ContactSubmissionResult
{
    public bool Success { get; init; }
    public string? Error { get; init; }
    public int SubmissionId { get; init; }
}