using System.ComponentModel.DataAnnotations;

namespace EventEase.Models
{
    
public class Venue
    {
        public int VenueId { get; set; }

        [Required(ErrorMessage = "Venue name is required.")]
        [StringLength(255, ErrorMessage = "Venue name cannot exceed 100 characters.")]
        public  string VenueName { get; set; }

        public bool IsAvailable { get; set; } //new fiels to track availability

        [Required(ErrorMessage = "Location is required.")]
        [StringLength(200, ErrorMessage = "Location cannot exceed 200 characters.")]
        public string Location { get; set; }

        [Required(ErrorMessage = "Capacity is required.")]
        [Range(1, 100000, ErrorMessage = "Capacity must be between 1 and 100,000.")]
        public int Capacity { get; set; }

        [Required(ErrorMessage = "An image is required.")]
        public  string ImageUrl { get; set; }

        public ICollection<Event> Events { get; set; } = new List<Event>();
    }

}
