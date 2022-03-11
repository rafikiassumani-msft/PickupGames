#nullable disable

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PickUpGames.Models;

    public class User
    {
        public int UserId {  get; set; }
        public string FirstName {  get; set; }
        public string LastName {  get; set; }
        public string Email {  get; set; }
        [JsonIgnore]
        public string PasswordHash {get; set;}
        public string DateOfBirth { get; set; }
        public string ProfileImageUrl {  get; set; }
        public DateTime CreatedAt {  get; set; } = DateTime.Now;
        public Nullable<DateTime> LastUpdatedAt {  get; set; }
        public IEnumerable<Event> Events {  get; set; }
    }

    public class UserDTO  {
        public int UserId {  get; set; }
        public string FirstName {  get; set; }
        public string LastName {  get; set; }
        public string Email {  get; set; }
        public string DateOfBirth { get; set; }
        public string ProfileImageUrl { get; set; }
         public DateTime CreatedAt {  get; set; }
         public Nullable<DateTime> LastUpdatedAt {get; set;}
         public IEnumerable<EventDTO> Events {get; set;}
    }

    public class UserRequestDTO  {
        [Required]
        public string FirstName {  get; set; }
        [Required]
        public string LastName {  get; set; }
        [Required]
        public string Email {  get; set; }
        [Required]
        public string Password {get; set;}
        [Required]
        public string DateOfBirth { get; set; }
        public string ProfileImageUrl { get; set; }
    }

#nullable restore
