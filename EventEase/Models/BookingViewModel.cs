namespace EventEase.ViewModels
{
    public class ManageBookingsViewModel
    {
        public string? Search { get; set; } //Primary key
        public IEnumerable<EventEase.Models.Booking> Bookings { get; set; } = [];
    }
}
