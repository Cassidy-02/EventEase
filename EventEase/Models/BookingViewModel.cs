namespace EventEase.ViewModels
{
    public class ManageBookingsViewModel
    {
        public string? Search { get; set; }
        public IEnumerable<EventEase.Models.Booking> Bookings { get; set; } = Enumerable.Empty<EventEase.Models.Booking>();
    }
}
