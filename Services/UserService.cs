
using PickUpGames.Models;

namespace PickUpGames.Services;

public class UserService : IUserService {

    private readonly PickupGamesDBContext _context;

    public UserService(PickupGamesDBContext dBContext) {  
        _context = dBContext;
    }

    public UserDto RegisterUser(UserDto userDto) {

        var user = MapUserData(userDto);
        _context.Add<User> (user);
        _context.SaveChanges();
        userDto.UserId = user.UserId;
         return userDto;
    }

    private string PasswordDigest(string password) {

        var enhancedHashPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(password);
        return enhancedHashPassword;
    }

    public (bool, User?) Authenticate(UserAuth userAuth) {

        var user = _context.Users.FirstOrDefault(u => u.Email == userAuth.Email);

        if(user == null) {
            return (false, null);
        }

        bool isVerified = BCrypt.Net.BCrypt.EnhancedVerify(userAuth.Password, user.PasswordHash);

        return (isVerified, user);
    }

    public User MapUserData(UserDto userDto) {

        return new User {
            FirstName = userDto.FirstName,
            LastName = userDto.LastName,
            Email = userDto.Email,
            PasswordHash = PasswordDigest(userDto.Password),
            DateOfBirth = userDto.DateOfBirth,
            ProfileImageUrl = userDto.ProfileImageUrl, // Need to change this to upload to azure blob and store azure blog url
            CreatedAt =  userDto.CreatedAt,
            LastUpdatedAt = null
        };
    }

}

public interface IUserService {

   public UserDto RegisterUser(UserDto userDto);

   public (bool, User?) Authenticate(UserAuth userAuth);

}