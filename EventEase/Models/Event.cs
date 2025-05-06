using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventEase.Models
{
    public class Event
    {
        public int EventId { get; set; }
        public string EventName { get; set; } = string.Empty; // Initialize with a default value
        public DateTime EventDate { get; set; }
        public string Description { get; set; } = string.Empty; // Initialize with a default value

        public int? VenueId { get; set; } // Foreign Key (nullable OK)
        public Venue? Venue { get; set; } // Navigation property

        [NotMapped]
        public List<SelectListItem> Venues { get; set; } = new(); // Initialize with an empty list
    }
}