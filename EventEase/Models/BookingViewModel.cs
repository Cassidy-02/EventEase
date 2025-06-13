using EventEase.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EventEase.ViewModels
{
    public class ManageBookingsViewModel
    {
        public string? Search { get; set; } //Primary key

        public List<Booking> Bookings { get; set; }

        public int? EventTypeId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public bool? IsAvailable { get; set; }

        public List<SelectListItem> EventTypes { get; set; } = new List<SelectListItem>();

    }
}
