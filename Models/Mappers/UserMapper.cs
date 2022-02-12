using Models.Mappers;

namespace PickUpGames.Models.Mappers;

public static class UserMapper {

    public static List<UserDTO> MapUsers(List<User> users) {

        var mappedUserList = new List<UserDTO> ();
        foreach(var user in users) {
            var userDto = MapUser(user);
            if(userDto is not null) {
                mappedUserList.Add(userDto);
            }
        }

        return mappedUserList;
    }

    public static UserDTO? MapUser(User? user) {

        if(user is not null) {
            return new UserDTO {
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                DateOfBirth = user.DateOfBirth,
                ProfileImageUrl = user.ProfileImageUrl,
                CreatedAt =  user.CreatedAt,
                Events = EventMapper.MapEvents(user.Events)
          };
        }

        return default;
    }

    
    public static UserDTO? MapUserWithNoEvent(User? user) {

        if(user is not null) {
            return new UserDTO {
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                DateOfBirth = user.DateOfBirth,
                ProfileImageUrl = user.ProfileImageUrl,
                CreatedAt =  user.CreatedAt,
          };
        }

        return default;
    }
}