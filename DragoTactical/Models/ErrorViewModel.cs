namespace DragoTactical.Models
{
    //------------------------------------------------------------------------------------------------------
    // Error View Model - View model for error page
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
//-------------------------------------------------<<< Endof File >>>----------------------------------------------------
