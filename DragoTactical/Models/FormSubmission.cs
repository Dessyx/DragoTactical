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

    [Required(ErrorMessage = "First name is required")]
    [StringLength(100, ErrorMessage = "First name cannot exceed 100 characters")]
    public string FirstName { get; set; } = null!;

    [Required(ErrorMessage = "Last name is required")]
    [StringLength(100, ErrorMessage = "Last name cannot exceed 100 characters")]
    public string LastName { get; set; } = null!;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters")]
    public string Email { get; set; } = null!;

    [StringLength(50)]
    [RegularExpression(@"^\+?[0-9\s\-()]{7,15}$", ErrorMessage = "Invalid phone number")]
    public string? PhoneNumber { get; set; }

    [StringLength(100)]
    [RegularExpression(@"^[^<>]{0,100}$", ErrorMessage = "Invalid company name")]
    public string? CompanyName { get; set; }

    [StringLength(100)]
    public string? Location { get; set; }

    public int? ServiceId { get; set; }

    [StringLength(3000, ErrorMessage = "Message cannot exceed 3000 characters")]
    [RegularExpression(@"^[^<>]{0,3000}$", ErrorMessage = "Invalid message")]
    public string? Message { get; set; }

    public DateTime SubmissionDate { get; set; }

    public virtual Service? Service { get; set; }
}
