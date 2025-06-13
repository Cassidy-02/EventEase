using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventEase.Models
{
    public class Event
    {
        public int EventId { get; set; }
        public string EventName { get; set; } = string.Empty; 
        public DateTime EventDate { get; set; }
        public string Description { get; set; } = string.Empty; 
        public int? VenueId { get; set; } // Foreign Key
        public Venue Venue { get; set; } // Navigation property
        public int? EventTypeId { get; set; } // Foreign Key
        public EventType EventType { get; set; } // Navigation property

        [NotMapped]
        public List<SelectListItem> Venues { get; set; } = new(); // Initialize with an empty list
        [NotMapped]
        public List<SelectListItem> EventTypes { get; set; } = new(); // Initialize with an empty list
    }
}