using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DragoTactical.Models;

public partial class FormSubmission
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
