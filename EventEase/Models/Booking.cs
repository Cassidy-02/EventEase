using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventEase.Models
{
    public class Booking
    {
        public int BookingId { get; set; }
        public int EventId { get; set; }
        public int VenueId { get; set; }
        [Required(ErrorMessage = "Booking date is required.")]
        public DateTime? BookingDate { get; set; }
        public required Event Event { get; set; }
        public required Venue Venue { get; set; }

        [NotMapped]
        // Add these properties for dropdown lists
        public List<SelectListItem>? Events { get; set; }
        [NotMapped]
        public List<SelectListItem>? Venues { get; set; }
    }

}
