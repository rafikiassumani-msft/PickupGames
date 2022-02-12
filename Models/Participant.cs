namespace PickUpGames.Models;
public class Participant
{
    public int ParticipantId { get; set; }    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime LastUpdatedAt {  get; set; }
    public Status Status {  get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public int EventId { get; set; }
    public Event? Event { get; set; }
}    

public class ParticipantDTO    
{
    public int ParticipantId { get; set; }    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime LastUpdatedAt {  get; set; }
    public Status Status {  get; set; }
    public UserDTO? User { get; set; }
    public int EventId { get; set; }
}

public enum Status
{
    Admitted,
    Rejected,
    WaitingList
}
