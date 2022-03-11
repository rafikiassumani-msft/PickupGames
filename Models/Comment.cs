#nullable disable

namespace PickUpGames.Models;

public class Comment {
    public int CommentId {get; set;}
    public string Message {get; set;}
    public int EventId {get; set;} 
    public Event Event {get; set;}
    public int UserId {get; set;}
    public User User {get; set;}
    public DateTime CreatedAt {get; set;} = DateTime.Now;
    public DateTime UpdatedAt {get; set;}
}

public class CommentDTO {
    public string Message {get; set;}
    public int EventId {get; set;} 
    public int UserId {get; set;}
}

#nullable enable