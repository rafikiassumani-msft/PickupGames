using System.ComponentModel.DataAnnotations;

namespace PickUpGames.Models;

public class UserAuth {
    
    [Required]
    public string? Email { get; set; }
    
    [Required]
    public string? Password {get; set;}
}