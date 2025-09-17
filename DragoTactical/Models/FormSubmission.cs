using System;
using System.Collections.Generic;

namespace DragoTactical.Models;

public partial class FormSubmission
{
    public int SubmissionId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public string? CompanyName { get; set; }

    public string? Location { get; set; }

    public int? ServiceId { get; set; }

    public string? Message { get; set; }

    public DateTime SubmissionDate { get; set; }
}
