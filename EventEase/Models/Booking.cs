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
        public  Event? Event { get; set; } 
        public  Venue? Venue { get; set; } 



        // Add these properties for dropdown lists
        [NotMapped]
        public List<SelectListItem> Events { get; set; } = new();
        [NotMapped]
        public List<SelectListItem> Venues { get; set; } = new();
        
    }

}
