namespace EventEase.Models
{
    public class Users
    {
        public int UserId { get; set; } //Primary Key
        public required string Username { get; set; }
        public required string PasswordHash { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
    }
}
