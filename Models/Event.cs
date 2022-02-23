#nullable disable

using System.Text.Json.Serialization;

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
    public int AddressId {get; set;}
    public Address Address { get; set; }
    public int MaxNumberOfParticipants { get; set; }
    public EventPrivacy EventPrivacy { get; set; }
    public EventStatus EventStatus { get; set; }
    public EventType EventType { get; set; }
    public IEnumerable<Participant> Participants { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? LastUpdatedAt { get; set; }
}

public class EventDTO 
{
    public int EventId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string StartDate { get; set; }
    public string StartTime { get; set; }
    public Address Location { get; set; }
    public int MaxNumberOfParticipants { get; set; }
    public string EventPrivacy { get; set; }
    public string EventStatus { get; set; }
    public string EventType { get; set; }
    public string Website {get; set;}
    public IEnumerable<ParticipantDTO> Participants { get; set; }
    public UserDTO Owner { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? LastUpdatedAt { get; set; }
}

public class EventRequestDTO 
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string StartDate { get; set; }
    public string StartTime { get; set; }
    public Address Location { get; set; }
    public int MaxNumberOfParticipants { get; set; }
    public int EventPrivacy { get; set; }
    public int EventStatus { get; set; }
    public int EventType { get; set; }
    public int OwnerId { get; set; }
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