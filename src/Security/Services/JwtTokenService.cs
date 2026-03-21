using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Security;
using Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Security.Options;

namespace Security.Services;

/// <summary>
/// Service for generating JWT access tokens.
/// </summary>
public class JwtTokenService(IOptions<JwtOptions> jwtOptions) : ITokenService
{
    /// <inheritdoc/>
    public AccessTokenResult GenerateAccessToken(User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        var options = jwtOptions.Value;
        var expiresAtUtc = DateTime.UtcNow.AddMinutes(options.AccessTokenExpirationMinutes);

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new(ClaimTypes.Name, user.Name),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Rol.ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expiresAtUtc,
            Issuer = options.Issuer,
            Audience = options.Audience,
            SigningCredentials = credentials
        };

        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
        var accessToken = tokenHandler.WriteToken(securityToken);

        return new AccessTokenResult(accessToken, expiresAtUtc, "Bearer");
    }
}
