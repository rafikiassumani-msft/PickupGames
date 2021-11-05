#nullable disable
namespace PickUpGames.Models;

public class Event 
{
    public int EventId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string StartDate { get; set; }
    public string EndDate { get; set; }
    public string StartTime { get; set; }
    public string EndTime { get; set; }
    public string Location { get; set; }
    public int MaxNumberOfParticipants { get; set; }
    public EventPrivacy EventPrivacy { get; set; }
    public EventStatus EventStatus { get; set; }
    public EventType EventType { get; set; }
    public IEnumerable<Participant> Participants { get; set; }
    public int UserId { get; set; }
    public User User {  get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime LastUpdatedAt { get; set; }
}

public enum EventPrivacy
{
    Private,
    Public,
    InviteOnly
}

public enum EventStatus
{
    Open,
    Closed,
    Cancelled
}

public enum EventType
{
    Soccer,
    Football,
    Baseball,
    Meeting
}

#nullable restore