using System;                        // imports
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DragoTactical.Models;

//------------------------------------------------------------------------------------------------------
// Service Model - Represents services offered
public partial class Service
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ServiceId { get; set; }                  // Variables

    public string? Title { get; set; }

    public string ServiceName { get; set; } = null!;

    public int CategoryId { get; set; }

    public string? Description { get; set; }

    public virtual Category Category { get; set; } = null!;
}
//-------------------------------------------------<<< Endof File >>>----------------------------------------------------
