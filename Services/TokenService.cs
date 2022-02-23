
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using PickUpGames.Models;

namespace PickUpGames.Services;

public class TokenService : ITokenService {

    private readonly IConfiguration _configuration; 
    private readonly JwtSecurityTokenHandlerFactory _jwtHandlerFactory;
    private readonly ILogger<ITokenService> _logger;
    
    public TokenService(IConfiguration configuration, 
                        JwtSecurityTokenHandlerFactory jwtSecurityTokenHandler, 
                        ILogger<ITokenService> logger) {

        _configuration = configuration;
        _jwtHandlerFactory = jwtSecurityTokenHandler;
        _logger = logger;
    }

    public string GenerateToken(User user) {

        var jwtSecurityTokenHandler = _jwtHandlerFactory.CreateInstance();
        var jwtToken = jwtSecurityTokenHandler.CreateToken(GetSecurityTokenDescriptor(user));

        return jwtSecurityTokenHandler.WriteToken(jwtToken);
    }

    private static IDictionary<string, object> GetClaims(User user) {
        var Claims = new Dictionary<string, object>() {
            {"firstName", user.FirstName},
            {"lastName", user.LastName},
            {"email", user.Email},
            {"can:create", "event"},
            {"can:delete", "event"},
            {"can:modifiy", "event"}
        };
        return Claims;
    }

    private SecurityTokenDescriptor GetSecurityTokenDescriptor(User user) {

        return new SecurityTokenDescriptor {
            Issuer = _configuration["JwtSettings:Issuer"],
            Audience = _configuration["JwtSettings:Audience"],
            Subject = new ClaimsIdentity( new[] { new Claim ("userId", user.UserId.ToString())}),
            Expires = DateTime.UtcNow.AddHours(3),
            IssuedAt = DateTime.UtcNow,
            Claims = GetClaims(user), 
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(GetKey()),  SecurityAlgorithms.HmacSha256Signature)
        };
    }

    public ClaimsPrincipal? ValidateToken(string jwtToken) {

        try {
              return _jwtHandlerFactory.CreateInstance().ValidateToken(jwtToken, new TokenValidationParameters
                    {
                        ValidateAudience = true,
                        ValidAudience = _configuration["JwtSettings:Audience"],
                        ValidateIssuer = true,
                        ValidIssuer = _configuration["JwtSettings:Issuer"],
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,
                        IssuerSigningKey = new SymmetricSecurityKey(GetKey())
                    }, out var validatedJwtToken);
        } catch (Exception ex) {
            _logger.LogError(ex, "Failed to validate jwt token");
        }

        return null;
    }

    private byte[] GetKey() {
        var ascii = Encoding.ASCII;
        return ascii.GetBytes(_configuration["JwtSettings:Key"]);
    }
}


public interface ITokenService {
   public string GenerateToken(User user);
   public ClaimsPrincipal? ValidateToken(string token);
}

public class  JwtSecurityTokenHandlerFactory {
    public JwtSecurityTokenHandler CreateInstance() {
        return new JwtSecurityTokenHandler();
    }
}