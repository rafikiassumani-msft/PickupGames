namespace PickUpGames.Models.Mappers;

public static class UserMapper {

    public static List<UserDto> MapUsers(List<User> users) {

        var mappedUserList = new List<UserDto> ();
        foreach(var user in users) {
            var userDto = MapUser(user);
            mappedUserList.Add(userDto);
        }

        return mappedUserList;
    }

    public static UserDto MapUser(User user) {
        return new UserDto {
            UserId = user.UserId,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            DateOfBirth = user.DateOfBirth,
            ProfileImageUrl = user.ProfileImageUrl,
            CreatedAt =  user.CreatedAt
        };
    }
}