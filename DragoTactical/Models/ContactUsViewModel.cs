using System.Collections.Generic;

namespace DragoTactical.Models
{
    public class ContactUsViewModel
    {
        public IEnumerable<Service> AllServices { get; set; } = new List<Service>();
    }
}

