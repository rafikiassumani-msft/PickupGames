#nullable disable

using System.Text.Json.Serialization;

namespace PickUpGames.Models;

    public class User
    {
        public int UserId {  get; set; }
        public string FirstName {  get; set; }
        public string LastName {  get; set; }
        public string Email {  get; set; }
        public string PasswordHash {get; set;}
        public string DateOfBirth { get; set; }
        public string ProfileImageUrl {  get; set; }
        public DateTime CreatedAt {  get; set; } = DateTime.Now;
        public Nullable<DateTime> LastUpdatedAt {  get; set; }
        public IEnumerable<Event> Events {  get; set; }
    }

    public class UserDto  {
        public int UserId {  get; set; }
        public string FirstName {  get; set; }
        public string LastName {  get; set; }
        public string Email {  get; set; }

        [JsonIgnore]
        public string Password {get; set;}
        public string DateOfBirth { get; set; }
        public string ProfileImageUrl { get; set; }
         public DateTime CreatedAt {  get; set; } = DateTime.Now;
    }

#nullable restore
