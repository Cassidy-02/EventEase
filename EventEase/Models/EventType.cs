namespace EventEase.Models
{
    public class EventType
    {
        public int EventTypeId { get; set; } // Primary Key
        public string EventTypeName { get; set; }
        // Navigation property for related events
        public ICollection<Event> Events { get; set; } 
    }
}
