using System.Collections.Generic;        // imports

namespace DragoTactical.Models
{
    //------------------------------------------------------------------------------------------------------
    // Contact Us View Model - View model for contact us page
    public class ContactUsViewModel
    {
        public IEnumerable<Service> AllServices { get; set; } = new List<Service>();
    }
}
//-------------------------------------------------<<< Endof File >>>----------------------------------------------------